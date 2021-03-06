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
using colorpicker;

namespace botof37s.Modules
{


    public class Leaderboardcommand : ModuleBase
    {
        public IConfiguration _config { get; set; }
        public DiscordSocketClient _client { get; set; }
        public DiscordRestClient rclient { get; set; }

        [Command("ranking")]
        [Alias("leaderboard")]
        [Summary("Displays the leaderboard")]
        [Name("📑 ranking")]
        [Remarks("all")]
        public async Task LeaderboardCommand()
        {
            
            DirectoryInfo di = new DirectoryInfo("leaderboard");
            Dictionary<long, int> temp= new Dictionary<long, int>();
            List<Tuple<string,int>> d =  new List<Tuple<string, int>>();
            foreach(FileInfo file in di.GetFiles())
            {
                temp.Add(long.Parse(file.Name.Replace(".37","")),int.Parse(File.ReadAllText($"leaderboard/{file.Name}"))); 
            }
            foreach(KeyValuePair<long,int> kvp in temp)
            {
                string username = null;
                
                var user = _client.GetUser((ulong)kvp.Key);

                if (user == null)
                {
                    var farsearch = await rclient.GetUserAsync((ulong)kvp.Key);
                    if(farsearch == null)
                    {
                        username = "Unknown User";
                    }
                    else
                    {
                        username = farsearch.Username;
                    }
                }
                else
                {
                    username = user.Username;
                }
                Tuple<string, int> entry = new Tuple<string, int>(username,kvp.Value);
                d.Add(entry);
            }
            var leaderboard = from entry in d orderby entry.Item2 descending select entry;
            EmbedBuilder builder = new EmbedBuilder();
            string lb = "";
            int b = 1;
            builder.WithAuthor("37Gang-Leaderboard", "https://cdn.discordapp.com/app-icons/737060692527415466/c64109fbdff1a1f6dfd7515eaec5198d.png?size=512", "https://bit.ly/37status");
            builder.WithFooter("Accuracy of these values can not be guaranteed", "https://cdn.discordapp.com/avatars/329650083819814913/33b46ac7c4bfa97c6df65b108fd8c008.png?size=512");
            foreach (Tuple<string, int> kvp in leaderboard)
            {
                string count;
                if (kvp.Item2 == 1)
                    count = "37";
                else
                    count = "37s";
                lb = lb + "\n" + b + $". {kvp.Item1} ({kvp.Item2} {count})";
                b++;
            }
            Colorpicker picker = new Colorpicker();
            builder.WithColor((uint)picker.Pick());
            builder.WithDescription(lb);
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}