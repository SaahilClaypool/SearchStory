# [Search Story](https://saapool.one/searchstory)

https://saapool.one/searchstory

> NOTE: this instance is readonly - I've seeded it with a few research papers on TCP and swarm intelligence.
> Try searching "bbr" or "tcp congestion control" or "novelty search"

[Lucene](https://lucene.apache.org/) based history searcher.

There are two use cases:

1. Upload files from your local machine to be indexed by lucene.

    If the files are text based (markdown, text, etc), or they are PDF files, they will be indexed properly.
    In the future, using [pandoc] to convert everything to markdown (word etc.) may more sustainable.
    I see this being used to index something like a zotero folder to search research papers you have read.

    I (and my one user...) provide the path to our [zotero] citation library to get full text search over all of the research articles we read.

2. Using the inclulded browser extension, save a "text only" version of the page to the lucene index.

    I see this being the main use case - any time you read an interesting thing, hit a shortcut and index the entire text content.
    This let's you have full text search and a copy of every interesting page you read.

![Example](./example.gif)

## Deployment

This is currently deployed in a small digital ocean droplet behind an NGINX reverse proxy.

## Development Ideas

- [in progress] Browser extension
    - [x] Uses [readability] to parse and save a readable page view
    - [x] Posts data to backend
    - [ ] Published in chrome store
    - [x] Visual feedback on article save
    - [ ] Pull video transcripts?
    - [ ] Allows a password and site url to be added (to be used with DO droplet etc.)
- [x] Cross platform app
    - [x] Build for windows & linux
    - [x] Writes to appropriate temporary directories
    - [x] True single-file exectuable
    - [x] Close to system tray / run in background
        
        May require hosting as a "real app" rather than console. Or at least a powershell wrapper for windows.
        
        (Windows only currently - cross platform could be added)

    - [ ] use blazor webview

[pandoc]: https://pandoc.org/using-the-pandoc-api.html
[readability]: https://github.com/mozilla/readability
[zotero]: https://www.zotero.org/
