using ModCore.Database;
using ModCore.Database.DatabaseEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ModCore.HansTagImport
{
    public class Importer
    {
        private List<HansTag> _loadedTags = new List<HansTag>();

        public void LoadTags(Stream tagStream)
        {
            var serializer = new JsonSerializer();
            using var streamReader = new StreamReader(tagStream);
            using var jsonReader = new JsonTextReader(streamReader);
            _loadedTags = serializer.Deserialize<List<HansTag>>(jsonReader);
        }

        public async Task DumpToDatabase(DatabaseContext database)
        {
            if(_loadedTags == default(List<HansTag>))
            {
                throw new InvalidOperationException("No tags were loaded!");
            }

            foreach(var tag in _loadedTags)
            {
                var latestRevision = tag.Revisions.FirstOrDefault(x => x.Created == tag.LatestRevision);

                // check if tag exists
                var existsTag = database.Tags.Any(x => x.Name == tag.Name && x.GuildId == (long)tag.Guild && x.ChannelId == (long)(tag.Channel ?? 0));

                var modcoreTag = existsTag? database.Tags.FirstOrDefault(x => x.Name == tag.Name && x.GuildId == (long)tag.Guild && x.ChannelId == (long)(tag.Channel ?? 0)) 
                    : new DatabaseTag();

                modcoreTag.Name = tag.Name;
                modcoreTag.ChannelId = (long)(tag.Channel ?? 0);
                modcoreTag.Contents = latestRevision.Contents;
                modcoreTag.CreatedAt = latestRevision.Created.DateTime;
                modcoreTag.GuildId = (long)tag.Guild;
                modcoreTag.OwnerId = (long)tag.Owner;

                if(existsTag)
                    database.Tags.Update(modcoreTag);
                else
                    database.Tags.Add(modcoreTag);
            }

            await database.SaveChangesAsync();
        }
    }
}
