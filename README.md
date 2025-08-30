# Jellyfin AnimeThemes Backup Metadata Plugin
## About
This plugin retrives metadata for the backup of [AnimeThemes](https://animethemes.moe).

> [!NOTE]
> This plugin is mostly for my personal use, it's not garanteed to work on all conditions.

## Install
TODO

## Use
Add the metadata provider to a `Music Videos` library where the AnimeThemes backup is stored.

> [!IMPORTANT]
> The filename from the backup should not be altered. This is crucial for the plugin to be able to find metadata.

## Development
To setup the development environment you can user `devcontainers`. The project should install all dependencies, clone and build the jellyfin repositories to be able to debug.

The Jellyfin branch can be configured in `.devcontainers/devcontainers.json` modifying the remote env `JELLYFIN_BRANCH`.

By default `$HOME/Videos/MusicVideos` gets mounted on `/home/vscode/MusicVideos` to setup your test library.
