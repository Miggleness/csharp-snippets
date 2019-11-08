# csharp-snippets

Reusable code snippets with minimal dependency.

## Database

**Linear Block Id Generator** - A sequence Id generator which works similarly with the Hi-Lo algorithm. A block of sequence numbers are reserved to the client by given sequence length and a Hi number returned by the database server. This article explains this in detail http://literatejava.com/hibernate/linear-block-allocator-a-superior-alternative-to-hilo/.

## File Operations

**WriteFileAsync** - Samples for writing a stream or text to file asynchronously.

# Using Series

## System.Threading.Channels

Resources:

- [Sacha Barber's blog](https://sachabarbs.wordpress.com/2018/11/28/system-threading-channels/)
- [Szymon Kulec's blog](https://blog.scooletz.com/2019/01/28/channels-disruptors-and-logs/)
- [Microsoft's docs](https://docs.microsoft.com/en-us/dotnet/api/system.threading.channels.channel.createbounded?view=dotnet-plat-ext-2.2)
- [Github Source](https://github.com/dotnet/corefx/tree/master/src/System.Threading.Channels/src/System/Threading/Channels)
- [Michael Shpilt's blob](https://michaelscodingspot.com/c-job-queues/) - haven't gone thru this one but looks promising
