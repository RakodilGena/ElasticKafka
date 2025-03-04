using Elastic.Clients.Elasticsearch;
using StorageService.Elastic;
using StorageService.Elastic.Models;
using StorageService.Messages.Mapping;
using StorageService.Messages.Models;
using StorageService.Messages.Models.Requests;

namespace StorageService.Messages.Services.Impl;

internal sealed class SearchMessagesService : ISearchMessagesService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<SearchMessagesService> _logger;

    public SearchMessagesService(
        ElasticsearchClient client,
        ILogger<SearchMessagesService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IReadOnlyList<MessageDto>> SearchMessagesAsync(
        SearchMessagesRequestDto request,
        CancellationToken cancellationToken)
    {
        var count = request.Count;
        var offset = request.Offset;
        var filter = request.Filter;

        _logger.LogInformation(
            "Searching messages from Elasticsearch [Count: {count}, Offset: {offset}, Filter: {filter}]",
            count,
            offset,
            filter);

        var response = await QueryMessagesAsync(request, cancellationToken);

        if (response.IsSuccess())
        {
            var fetchedCount = response.Documents.Count;
            _logger.LogInformation("Successfully searched messages, count = {count}",
                fetchedCount);

            return response.Documents.Select(d => d.ToDto()).ToArray();
        }


        _logger.LogError("Failed to search messages");
        //any other response
        //will raise an exception
        throw new Exception(response.DebugInformation);
    }



    private async Task<SearchResponse<ElasticMessage>> QueryMessagesAsync(
        SearchMessagesRequestDto request,
        CancellationToken cancellationToken)
    {
        // var searchTerms = request.Filter.Split(' ',
        //     StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        //
        // _logger.LogInformation("Discovered search terms: {terms}",
        //     string.Join(", ", searchTerms));

        var response = await _client.SearchAsync<ElasticMessage>(
            s => s
                .Index(ElasticIndices.Messages)
                .Size(request.Count) // Limit results
                .From(request.Offset) // Skip results
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Text)
                        .Query(request.Filter.ToLower())
                    )
                )
                .Sort(sort => sort.Field(
                        f => f.SentAt,
                        f => f.Order(SortOrder.Desc)) // Newest first

                ), cancellationToken);

        return response;
    }
}

//all that below didn't work. all tries led to elastic ignoring all the search terms
//execept the last one applied...

// internal static class Ext
// {
//     public static SearchRequestDescriptor<ElasticMessage> JoinSearchTerms(
//         this SearchRequestDescriptor<ElasticMessage> d,
//         string[] searchTerms)
//     {
//         foreach (var term in searchTerms)
//         {
//             d = d.Query(q => q
//                 .Match(m => m
//                     .Field(f => f.Text)
//                     .Query(term)
//                 ));
//         }
//     
//         return d;
//     }
//
//     public static MatchQueryDescriptor<ElasticMessage> JoinSearchTerms(
//         this MatchQueryDescriptor<ElasticMessage> d,
//         string[] searchTerms)
//     {
//         foreach (var term in searchTerms)
//         {
//             d = d.Field(f => f.Text).Query(term);
//         }
//     
//         return d;
//     }
// }

// private async Task<SearchResponse<ElasticMessage>> QueryMessagesAsync(
//     SearchMessagesRequestDto request,
//     CancellationToken cancellationToken)
// {
//     var searchTerms = request.Filter.Split(' ',
//         StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
//
//     _logger.LogInformation("Discovered search terms: {terms}",
//         string.Join(", ", searchTerms));
//
//     var response = await _client.SearchAsync<ElasticMessage>(
//         s => s
//             .Index(ElasticIndices.Messages)
//             .Size(request.Count) // Limit results
//             .From(request.Offset) // Skip results
//             // .Query(q => q
//             //     .Bool(b => b.Must(mb =>
//             //     {
//             //         foreach (var term in searchTerms)
//             //         {
//             //             //case-insensitive
//             //             mb.Match(m => m
//             //                 .Field(f => f.Text)
//             //                 .Query(term)
//             //                   DOESN'T WORK, IGNORES ALL TERMS EXCEPT LAST ONE
//             //             );
//             //         }
//             //     }))
//             // )
//             //for simple match
//             //DOESN'T WORK, RETURNS RESULT IF ANY TERM MATCHES
//             // .Query(q => q
//             //     .Match(m => m
//             //             .Field(f => f.Text)
//             //             .Query(request.Filter)
//             //     
//             //     )
//             // )
//             .Sort(sort => sort.Field(
//                     f => f.SentAt,
//                     f => f.Order(SortOrder.Desc)) // Newest first
//
//             ), cancellationToken);
//
//     return response;
// }