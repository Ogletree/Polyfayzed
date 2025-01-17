using System.Text.Json;
using AspireApp.ApiService.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace AspireApp.ApiService.Services;

public class TeamUpdateService(PolyfayzedContext context, HttpClient httpClient, JsonSerializerOptions jsonOptions)
{
    private const int BatchSize = 100;

    [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public async Task IntegrateAsync()
    {
            var teams = await FetchTeams();

            var existingTeams = new List<Team>();
            for (int i = 0; i < teams.Count; i += BatchSize)
            {
                var batch = teams.Skip(i).Take(BatchSize).Select(m => m.Id).ToList();
                var batchExistingTeams = await context.Teams
                    .Where(team => batch.Contains(team.Id))
                    .ToListAsync();
                existingTeams.AddRange(batchExistingTeams);
            }

            var existingIds = existingTeams.Select(m => m.Id).ToHashSet();
            var newTeams = teams.Where(m => !existingIds.Contains(m.Id)).ToList();
            if (newTeams.Any())
            {
                await using var transaction = await context.Database.BeginTransactionAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Team ON");
                context.Teams.AddRange(newTeams);
                await context.SaveChangesAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Team OFF");
                await transaction.CommitAsync();
            }

            foreach (var existingTeam in existingTeams)
            {
                var updatedTeam = teams.First(m => m.Id == existingTeam.Id);
                updatedTeam.Abbreviation = existingTeam.Abbreviation;
                updatedTeam.CreatedAt = existingTeam.CreatedAt;
                updatedTeam.UpdatedAt = existingTeam.UpdatedAt;
                updatedTeam.Name = existingTeam.Name;
                updatedTeam.Record = existingTeam.Record;
                updatedTeam.Logo = existingTeam.Logo;
                updatedTeam.Alias = existingTeam.Alias;
                updatedTeam.League = existingTeam.League;
                context.Entry(existingTeam).CurrentValues.SetValues(updatedTeam);
            }

            await context.SaveChangesAsync();
    }

    private async Task<List<Team>> FetchTeams()
    {
        var url = "https://gamma-api.polymarket.com/teams?limit=2000";
        var response = await httpClient.GetStringAsync(url);
        return JsonSerializer.Deserialize<List<Team>>(response, jsonOptions);
    }
}