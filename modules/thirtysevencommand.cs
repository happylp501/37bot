﻿using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
using Discord.Rest;
using Microsoft.Extensions.Configuration.Json;
using botof37s;

namespace botof37s.Modules
{


    public class Thirtysevencommand : ModuleBase
    {
        
        [Command("/37")]
        [Alias("37")]
        public async Task ThirtysevenCommand()
        {
            Console.WriteLine("command called");
            DateTime last37 =new DateTime();
            IConfiguration _config;

            var _builder = new ConfigurationBuilder().
            SetBasePath(AppContext.BaseDirectory).
            AddJsonFile(path: "config.json");

            _config = _builder.Build();
            if (File.Exists("db/lastmessage.37"))
            {
                last37 = Convert.ToDateTime(File.ReadAllText("db/lastmessage.37"));
            }
            TimeSpan ts = DateTime.UtcNow - last37;
            if (ts.TotalMinutes >= Int32.Parse(_config["Frequency"]))
            {
                int personalcount = 0;
                if (File.Exists($"leaderboard/{Context.User.Id}.37"))
                {
                    personalcount = Int32.Parse(File.ReadAllText($"leaderboard/{Context.User.Id}.37"));
                }
                File.WriteAllText("db/lastmessage.37", DateTime.UtcNow.ToString());
                File.WriteAllText($"leaderboard/{Context.User.Id}.37", (personalcount + 1).ToString());
                File.WriteAllText("db/last37uname.37", Context.User.Username);
                cooldown cooldown = new cooldown();
                cooldown.CooldownAsync(int.Parse(_config["Frequency"]), (DiscordSocketClient)Context.Client);
                var replies = new List<string>();
                replies.Add($"<@{Context.User.Id}> Coming right up!");
                replies.Add($"@{Context.User.Id}> As you wish!");
                replies.Add($"@{Context.User.Id}> I cant believe its not spam!");
                replies.Add($"@{Context.User.Id}> Ugh, fine!");
                var answer = replies[new Random().Next(replies.Count - 1)];
                await Context.Channel.SendMessageAsync(answer);
            }
            else
            {
                string last37uname = "[REDACTED]";
                if (File.Exists("db/last37uname.37"))
                {
                    last37uname = File.ReadAllText("db/last37uname.37");
                }
                await Context.Channel.SendMessageAsync($"I'm sorry <@{Context.User.Id}>, but you will have to wait another {Math.Floor(Int32.Parse(_config["Frequency"]) - ts.TotalMinutes)} minutes and {60 - ts.Seconds} seconds. The last 37 was claimed by {last37uname}");
            }

        }
    }
    public class cooldown
    {
        public async Task CooldownAsync(int time, DiscordSocketClient _client)
        {
            await _client.SetStatusAsync(UserStatus.DoNotDisturb);
            await Task.Delay(1000 * 60 * time);
            await _client.SetStatusAsync(UserStatus.Online);
        }
    }


}