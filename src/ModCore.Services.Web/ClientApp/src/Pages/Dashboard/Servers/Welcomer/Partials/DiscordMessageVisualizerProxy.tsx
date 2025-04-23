import FakeDiscordMessage from "@/Components/FakeDiscordMessage";
import { DiscordApplication } from "@/Types/DiscordTypes/DiscordApplication";
import { DiscordEmbed } from "@/Types/DiscordTypes/DiscordEmbed";
import { User } from "@/Types/User";
import { usePage } from "@inertiajs/react";
import { useEffect, useState } from "react";

export default function DiscordMessageVisualizerProxy({
  embeds,
  content,
  className,
}: {
  embeds?: DiscordEmbed[];
  content: string;
  className: string;
}) {
  const [parsedEmbeds, setParsedEmbeds] = useState<DiscordEmbed[] | undefined>(
    embeds,
  );
  const [parsedContent, setParsedContent] = useState<string>(content);
  const { application, user } = usePage<{
    application: DiscordApplication;
    user: User;
  }>().props;

  function overridePlaceholder(input?: string): string {
    if (!input) return "";
    // switch case with examples
    switch (input) {
      case "username":
        return user.username;
      case "mention":
        return "<@1234567890>";
      case "userid":
        return "1234567890";
      case "guildname":
        return "ModCore's Server";
      case "channelname":
        return "general";
      case "membercount":
        return "100";
      case "owner-username":
        return "Owner";
      case "guild-icon-url":
        return `https://cdn.discordapp.com/app-icons/${application.id}/${application.icon}.png`;
      case "avatar":
        return user.avatar;
      case "channel-count":
        return "5";
      case "role-count":
        return "10";
      default:
        return input;
    }
  }

  function overrideAllPlaceholders(input?: string): string {
    if (!input) return "";
    return input.replace(/{{(.*?)}}/g, (match, p1) => overridePlaceholder(p1));
  }

  function overridePlaceholdersInEmbed(embed: DiscordEmbed): DiscordEmbed {
    const newEmbed = {
      ...embed,
      title: overrideAllPlaceholders(embed.title ?? ""),
      description: overrideAllPlaceholders(embed.description ?? ""),
      footer: embed.footer
        ? {
            text: overrideAllPlaceholders(embed.footer?.text ?? ""),
            icon_url: overrideAllPlaceholders(embed.footer?.icon_url ?? ""),
          }
        : undefined,
      author: embed.author
        ? {
            name: overrideAllPlaceholders(embed.author?.name ?? ""),
            icon_url: overrideAllPlaceholders(embed.author?.icon_url ?? ""),
            url: overrideAllPlaceholders(embed.author?.url ?? ""),
          }
        : undefined,
      image: embed.image
        ? {
            url: overrideAllPlaceholders(embed.image?.url ?? ""),
          }
        : undefined,
      thumbnail: embed.thumbnail
        ? {
            url: overrideAllPlaceholders(embed.thumbnail?.url ?? ""),
          }
        : undefined,
      fields: embed.fields?.map((field) => ({
        ...field,
        name: overrideAllPlaceholders(field.name),
        value: overrideAllPlaceholders(field.value),
      })),
    };

    return newEmbed;
  }

  useEffect(() => {
    if (embeds) {
      const newEmbeds = embeds.map((embed) =>
        overridePlaceholdersInEmbed(embed),
      );
      setParsedEmbeds(newEmbeds);
    }
    setParsedContent(overrideAllPlaceholders(content));
  }, [embeds, content]);

  return (
    <FakeDiscordMessage
      embeds={parsedEmbeds}
      content={parsedContent}
      className={className}
    />
  );
}
