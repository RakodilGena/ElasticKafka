using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Analysis;
using StorageService.Elastic;
using StorageService.Elastic.Models;

namespace StorageService.Migration;

public sealed class ElasticMigrator
{
    public static async Task MigrateAsync(WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<ElasticMigrator>>();
        var client = app.Services.GetRequiredService<ElasticsearchClient>();

        var messagesIndex = ElasticIndices.Messages;

        logger.LogInformation("Elasticsearch migrator started");
        logger.LogInformation("Creating index \"{idx}\"", messagesIndex);

        var existsResponse = await client.Indices.ExistsAsync(messagesIndex);

        if (existsResponse.Exists)
        {
            logger.LogInformation("Index \"{idx}\" already exists, skipping.", messagesIndex);
            
            //await client.Indices.DeleteAsync(messagesIndex);
            
            return;
        }

        var createIndexResponse = await client.Indices.CreateAsync(messagesIndex, c => c
            .Settings(s => s
                .MaxNgramDiff(4) //for ngrams NECESSARY

                .NumberOfShards(3) // Distribute data across 3 primary shards

                .NumberOfReplicas(2) // Keep 2 replicas for fault tolerance

                //todo add case insentiveness
                .Analysis(a => a
                    // if needed to replace some punctuation with spaces.    
                    // .CharFilters(cf => cf
                    //     .PatternReplace("punctuation_remover", pr => pr
                    //             .Pattern("[.,!?]") // Removes ., !, ?
                    //             .Replacement(" ")  // Replaces with a space
                    //     )
                    // )
                    .Tokenizers(t => t
                        .NGram("ngram_3_7_tokenizer", ng => ng
                            .MinGram(3)
                            .MaxGram(7)
                            .TokenChars([TokenChar.Letter, TokenChar.Digit, TokenChar.Whitespace])
                        )
                    )
                    //important for case-insensitive search
                    .TokenFilters(tf => tf
                            .Lowercase("lowercase_filter") // Add lowercase filter
                    )
                    .Analyzers(an => an
                        .Custom("ngram_3_7_analyzer", ca => ca
                            //.CharFilter(["punctuation_remover"])  // Apply punctuation filter
                            .Tokenizer("ngram_3_7_tokenizer")
                            .Filter(["lowercase_filter"])// Apply the lowercase filter
                        )
                    )
                )
            )
            .Mappings(m => m
                .Properties<ElasticMessage>(props =>
                    props
                        .Keyword(t => t.Id)
                        .Text(
                            t => t.Text,
                            t => t.Analyzer("ngram_3_7_analyzer"))
                        .Date(t => t.SentAt)
                        .Date(
                            t => t.SavedAt,
                            t => t.Store().Index(false))
                )
            )
        );


        if (createIndexResponse.IsValidResponse is false)
        {
            throw new Exception($"Failed to create index: {createIndexResponse.DebugInformation}");
        }

        logger.LogInformation("Successfully created index \"{idx}\"", messagesIndex);
    }
}