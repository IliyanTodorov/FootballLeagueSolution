using AutoMapper;
using FootballLeague.Data.Common.Repositories;
using FootballLeague.Data.Models;
using FootballLeague.Data.Models.DTOs;
using FootballLeague.Services;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Match = FootballLeague.Data.Models.Match;

namespace FootballLeague.Tests
{
    public class TeamServiceTests
    {
        private readonly Mock<IDeletableEntityRepository<Team>> teamRepoMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly TeamService teamService;

        public TeamServiceTests()
        {
            teamRepoMock = new Mock<IDeletableEntityRepository<Team>>();
            mapperMock = new Mock<IMapper>();
            teamService = new TeamService(teamRepoMock.Object, mapperMock.Object);
        }

        [Fact]
        public async Task GetAllTeamsAsync_ShouldReturnMappedTeamListDtos()
        {
            var teamList = new List<Team>
            {
                new Team { Id = 1, Name = "Team A" },
                new Team { Id = 2, Name = "Team B" }
            };

            
            var teamsMock = teamList.AsQueryable().BuildMock();

            
            teamRepoMock.Setup(r => r.All()).Returns(teamsMock.Object);

            
            mapperMock.Setup(m => m.Map<IEnumerable<TeamListDto>>(It.IsAny<IEnumerable<Team>>()))
                      .Returns((IEnumerable<Team> ts) => ts.Select(t => new TeamListDto { Id = t.Id, Name = t.Name }));

            var result = await teamService.GetAllTeamsAsync();

            Assert.NotNull(result);
            Assert.Equal(teamList.Count, result.Count());
            Assert.Collection(result,
                dto =>
                {
                    Assert.Equal(1, dto.Id);
                    Assert.Equal("Team A", dto.Name);
                },
                dto =>
                {
                    Assert.Equal(2, dto.Id);
                    Assert.Equal("Team B", dto.Name);
                });
        }

        [Fact]
        public async Task GetTeamByIdAsync_ShouldReturnTeamDto_WhenTeamExists()
        {
            var team = new Team { Id = 1, Name = "Team A" };
            var teamList = new List<Team> { team };

            var teamsMock = teamList.AsQueryable().BuildMock();
            teamRepoMock.Setup(r => r.All()).Returns(teamsMock.Object);

            mapperMock.Setup(m => m.Map<TeamResponseDto>(It.IsAny<Team>()))
                      .Returns((Team t) => new TeamResponseDto { Id = t.Id, Name = t.Name });

            var result = await teamService.GetTeamByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Team A", result.Name);
        }

        [Fact]
        public async Task GetTeamByIdAsync_ShouldReturnNull_WhenTeamDoesNotExist()
        {
            var teamList = new List<Team>();
            var teamsMock = teamList.AsQueryable().BuildMock();
            teamRepoMock.Setup(r => r.All()).Returns(teamsMock.Object);

            var result = await teamService.GetTeamByIdAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetTeamStatsAsync_ShouldReturnCorrectStats_WhenTeamExists()
        {
            // Arrange: Create a team with home and away matches.
            var team = new Team
            {
                Id = 1,
                Name = "Team A",
                HomeMatches = new List<Match>
                {
                    new Match { HomeTeamScore = 2, AwayTeamScore = 1 }, // win
                    new Match { HomeTeamScore = 1, AwayTeamScore = 1 }  // draw
                },
                AwayMatches = new List<Match>
                {
                    new Match { AwayTeamScore = 3, HomeTeamScore = 0 }, // win
                    new Match { AwayTeamScore = 0, HomeTeamScore = 2 }  // loss
                }
            };
            var teamList = new List<Team> { team };
            var teamsMock = teamList.AsQueryable().BuildMock();
            teamRepoMock.Setup(r => r.All()).Returns(teamsMock.Object);

            // Act
            var result = await teamService.GetTeamStatsAsync(1);

            // Assert:
            // Wins: 2 (one home win and one away win)
            // Draws: 1 (one home draw)
            // Losses: 1 (one away loss)
            // Points: 2*3 + 1*1 = 7
            Assert.NotNull(result);
            Assert.Equal(2, result.Wins);
            Assert.Equal(1, result.Draws);
            Assert.Equal(1, result.Losses);
            Assert.Equal(7, result.Points);
        }

        [Fact]
        public async Task GetTeamStatsAsync_ShouldThrowArgumentException_WhenTeamDoesNotExist()
        {
            // Arrange
            var teamList = new List<Team>();
            var teamsMock = teamList.AsQueryable().BuildMock();
            teamRepoMock.Setup(r => r.All()).Returns(teamsMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => teamService.GetTeamStatsAsync(99));
        }

        [Fact]
        public async Task CreateTeamAsync_ShouldAddTeamAndReturnTeamResponseDto()
        {
            // Arrange
            var createTeamDto = new CreateTeamDto { Name = "New Team" };
            var team = new Team { Id = 1, Name = "New Team" };

            mapperMock.Setup(m => m.Map<Team>(It.IsAny<CreateTeamDto>()))
                      .Returns(team);
            mapperMock.Setup(m => m.Map<TeamResponseDto>(It.IsAny<Team>()))
                      .Returns((Team t) => new TeamResponseDto { Id = t.Id, Name = t.Name });

            teamRepoMock.Setup(r => r.AddAsync(It.IsAny<Team>())).Returns(Task.CompletedTask);
            teamRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await teamService.CreateTeamAsync(createTeamDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("New Team", result.Name);
        }


        [Fact]
        public async Task UpdateTeamAsync_ShouldReturnTrue_WhenTeamExists()
        {
            // Arrange: Create an existing team.
            var existingTeam = new Team { Id = 1, Name = "Old Name" };
            var teamList = new List<Team> { existingTeam };
            var teamsMock = teamList.AsQueryable().BuildMock();
            teamRepoMock.Setup(r => r.All()).Returns(teamsMock.Object);

            var updateTeamDto = new UpdateTeamDto { Name = "New Name" };

            mapperMock.Setup(m => m.Map(updateTeamDto, existingTeam))
                      .Callback(() => { existingTeam.Name = updateTeamDto.Name; });

            teamRepoMock.Setup(r => r.Update(existingTeam));
            teamRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await teamService.UpdateTeamAsync(1, updateTeamDto);

            // Assert
            Assert.True(result);
            Assert.Equal("New Name", existingTeam.Name);
        }

        [Fact]
        public async Task UpdateTeamAsync_ShouldReturnFalse_WhenTeamDoesNotExist()
        {
            // Arrange
            var teamList = new List<Team>();
            var teamsMock = teamList.AsQueryable().BuildMock();
            teamRepoMock.Setup(r => r.All()).Returns(teamsMock.Object);

            var updateTeamDto = new UpdateTeamDto { Name = "Barcelona" };

            // Act
            var result = await teamService.UpdateTeamAsync(99, updateTeamDto);

            // Assert
            Assert.False(result);
        }

        
        [Fact]
        public async Task DeleteTeamAsync_ShouldReturnTrue_WhenTeamExists()
        {
            // Arrange
            var existingTeam = new Team { Id = 1, Name = "Team A" };
            var teamList = new List<Team> { existingTeam };
            var teamsMock = teamList.AsQueryable().BuildMock();
            teamRepoMock.Setup(r => r.All()).Returns(teamsMock.Object);
            teamRepoMock.Setup(r => r.Delete(existingTeam));
            teamRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await teamService.DeleteTeamAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteTeamAsync_ShouldReturnFalse_WhenTeamDoesNotExist()
        {
            // Arrange
            var teamList = new List<Team>();
            var teamsMock = teamList.AsQueryable().BuildMock();
            teamRepoMock.Setup(r => r.All()).Returns(teamsMock.Object);

            // Act
            var result = await teamService.DeleteTeamAsync(99);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetTeamRankingsAsync_ShouldReturnRankedTeams()
        {
            // Arrange
            var team1 = new Team
            {
                Id = 1,
                Name = "Manchester City",
                HomeMatches = new List<Match>
                {
                    new Match { HomeTeamScore = 2, AwayTeamScore = 0 } // win => 3 points
                },
                AwayMatches = new List<Match>()
            };
            var team2 = new Team
            {
                Id = 2,
                Name = "Manchester United",
                HomeMatches = new List<Match>(),
                AwayMatches = new List<Match>
                {
                    new Match { AwayTeamScore = 1, HomeTeamScore = 1 } // draw => 1 point
                }
            };
            var teamList = new List<Team> { team1, team2 };
            var teamsMock = teamList.AsQueryable().BuildMock();
            teamRepoMock.Setup(r => r.All()).Returns(teamsMock.Object);

            // Act
            var rankings = await teamService.GetTeamRankingsAsync();

            // Assert: Expect team1 (Manchester City) to rank first with 3 points.
            Assert.NotNull(rankings);
            var rankedList = rankings.ToList();
            Assert.Equal(2, rankedList.Count);
            Assert.Equal(1, rankedList[0].TeamId);      // team1
            Assert.Equal(3, rankedList[0].Points);      // win => 3 points
            Assert.Equal(2, rankedList[1].TeamId);      // team2
            Assert.Equal(1, rankedList[1].Points);      // draw => 1 point
        }
    }
}
