
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.EntityFrameworkCore;
using SnakeServerAPI.DataBase;
using SnakeServerAPI.DataBase.Data;

namespace SnakeServerAPI.DiscordApi
{
    public class CheckMemberRole
    {
        #region GUILD
        static readonly string token = "";
        static readonly ulong guildId = 123;
        #endregion


        private DiscordClient _discordClient;
        private IServiceProvider _serviceProvider;
       

        public CheckMemberRole(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            //_guild = guild;
            _discordClient = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });
        }

        
        public async Task Start()
        {
            await _discordClient.ConnectAsync();

            bool ready = false;
            _discordClient.Ready += async (s, e) =>
            {
                ready = true;
                return;
            };
            while (!ready)
            {
                await Task.Delay(100);
            }
            await _discordClient.UpdateStatusAsync(new DiscordActivity("SNAKES DAO\nhttps://discord.gg/R8pscs4S3G"), userStatus: UserStatus.DoNotDisturb);

            var Guild = await _discordClient.GetGuildAsync(guildId);

            var Members = await Guild.GetAllMembersAsync();
            using (var scope = _serviceProvider.CreateScope())
            {
                using var snakeDB = scope.ServiceProvider.GetService<SnakeDB>();
                foreach (var user in Members)
                {
                    var dbUserRoles = snakeDB.Roles.Where(p => user.Roles.Select(p => p.Id).Contains(p.RoleId)).ToList();
                    var dbUser = snakeDB.Users.Include(p => p.Roles).FirstOrDefault(p => p.Discord_Id == user.Id);

                    if (dbUser == null)
                    {
                        snakeDB.Users.Add(
                            new SnakeUser
                            {
                                Discord_Id = user.Id,
                                Roles = dbUserRoles,
                                Subscripted_At = DateTime.Now,
                                Ban = false,
                                Subsripted = true,
                                DS_Nick = $"{user.Username}#{user.Discriminator}"
                            });
                    }
                    else
                    {
                        dbUser.Roles = dbUserRoles;
                    }
                }
                await snakeDB.SaveChangesAsync();
            }

            _discordClient.GuildMemberUpdated += async (s, e) =>
            {
                using var scope = _serviceProvider.CreateScope();
                using var snakeDB = scope.ServiceProvider.GetService<SnakeDB>();
                var dbUserRoles = snakeDB.Roles.Where(p => e.RolesAfter.Select(p => p.Id).Contains(p.RoleId)).ToList();
                var dbUser = snakeDB.Users.Include(p => p.Roles).FirstOrDefault(p => p.Discord_Id == e.Member.Id);
                if (dbUser == null)
                {
                    snakeDB.Users.Add(
                        new SnakeUser
                        {
                            Discord_Id = e.Member.Id,
                            Subscripted_At = DateTime.Now,
                            Roles = dbUserRoles,
                            Ban = false,
                            Subsripted = true,
                            DS_Nick = $"{e.Member.Username}#{e.Member.Discriminator}"
                        });
                }
                else dbUser.Roles = dbUserRoles;

                await snakeDB.SaveChangesAsync();
            };
            await Task.Delay(Timeout.Infinite);
        }
    }
}

