namespace Cirreum.Runtime.Tests.Authentication.Enrichment;

using Cirreum;
using Cirreum.Runtime.Authentication.Enrichment;
using Cirreum.Security;
using AwesomeAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Graph.Models;
using NSubstitute;
using System.Security.Claims;
using Xunit;

public class CommonGraphProfileEnricherTests {

	private sealed class TestGraphEnricher(
		CommonGraphData data,
		IDateTimeClock clock)
		: CommonGraphProfileEnricher(NullLogger.Instance, clock) {

		protected override Task<CommonGraphData> QueryGraph() => Task.FromResult(data);
	}

	private static (UserProfile Profile, ClaimsIdentity Identity) ProfileWith(params Claim[] claims) {
		var identity = new ClaimsIdentity(claims, authenticationType: "test", nameType: "name", roleType: "roles");
		var profile = new UserProfile(new ClaimsPrincipal(identity), TimeZoneInfo.Utc.Id);
		return (profile, identity);
	}

	private static TestGraphEnricher EnricherWith(User graphUser) {
		var clock = Substitute.For<IDateTimeClock>();
		clock.LocalTimeZoneId.Returns(TimeZoneInfo.Utc.Id);
		clock.LocalOffset.Returns(DateTimeOffset.UtcNow);
		var data = new CommonGraphData(graphUser, new MailboxSettings(), [], [], null);
		return new TestGraphEnricher(data, clock);
	}

	[Fact]
	public async Task Graph_enrichment_does_not_overwrite_the_nickname_claim_with_the_mail_alias() {
		// Nickname is the OIDC nickname claim (a casual name); Graph's mailNickname is the
		// directory mail alias — a different concept that previously stomped the claim value.
		var (profile, identity) = ProfileWith(
			new Claim("tid", "tenant-1"),
			new Claim("oid", "user-1"),
			new Claim("nickname", "Gilly"));

		var enricher = EnricherWith(new User {
			DisplayName = "Glen Banta",
			MailNickname = "glen.banta"
		});

		await enricher.EnrichProfileAsync(profile, identity);

		profile.Nickname.Should().Be("Gilly");
		// The directory display name still wins the DisplayName slot when present.
		profile.DisplayName.Should().Be("Glen Banta");
	}

	[Fact]
	public async Task Graph_display_name_wins_over_a_claims_derived_value() {
		// The claims pass (ClaimsUserProfileEnricher, Cirreum.Domain — invoked as the first
		// step of EnrichProfileAsync) fills DisplayName from claims alone when possible. This
		// simulates that prior fill directly, rather than asserting Domain's own fallback
		// chain here — that logic is Domain's, and is covered by Domain's own test suite.
		var (profile, identity) = ProfileWith(
			new Claim("tid", "tenant-1"),
			new Claim("oid", "user-1"));
		profile.DisplayName = "Claims Derived Name";

		var enricher = EnricherWith(new User { DisplayName = "Directory Display Name" });

		await enricher.EnrichProfileAsync(profile, identity);

		profile.DisplayName.Should().Be("Directory Display Name");
	}

	[Fact]
	public async Task A_claims_derived_display_name_survives_when_the_directory_has_none() {
		// Lets a UI consolidate on DisplayName: when Graph returns no displayName (sparse
		// directory, or the Graph query failed), the claims-derived value is not cleared.
		var (profile, identity) = ProfileWith(
			new Claim("tid", "tenant-1"),
			new Claim("oid", "user-1"));
		profile.DisplayName = "Claims Derived Name";

		var enricher = EnricherWith(new User());

		await enricher.EnrichProfileAsync(profile, identity);

		profile.DisplayName.Should().Be("Claims Derived Name");
	}

	[Fact]
	public async Task The_mail_alias_never_masquerades_as_a_nickname_when_no_claim_exists() {
		var (profile, identity) = ProfileWith(
			new Claim("tid", "tenant-1"),
			new Claim("oid", "user-1"));

		var enricher = EnricherWith(new User {
			DisplayName = "Glen Banta",
			MailNickname = "glen.banta"
		});

		await enricher.EnrichProfileAsync(profile, identity);

		profile.Nickname.Should().BeNull();
	}
}
