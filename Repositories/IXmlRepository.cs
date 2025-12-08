using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.DataProtection.Repositories;

public class MongoXmlRepository : IXmlRepository
{
    private readonly IMongoCollection<BsonDocument> _collection;

    public MongoXmlRepository(IMongoCollection<BsonDocument> collection)
    {
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));
    }

    public IReadOnlyCollection<XElement> GetAllElements()
    {
        var docs = _collection.Find(FilterDefinition<BsonDocument>.Empty).ToList();
    
        // ログ出力（件数とキーの内容の一部を確認）
        Console.WriteLine($"MongoXmlRepository.GetAllElements 呼び出し: {docs.Count} 件のキーを読み込みました");
    
        foreach (var doc in docs)
        {
            Console.WriteLine($"キー: {doc.ToJson()}");
        }
    
        return docs
            .Select(d => XElement.Parse(d["Xml"].AsString))
            .ToList()
            .AsReadOnly();
    }

    public void StoreElement(XElement element, string friendlyName)
    {
        var doc = new BsonDocument
        {
            { "FriendlyName", friendlyName ?? string.Empty },
            { "Xml", element.ToString(SaveOptions.DisableFormatting) }
        };
        _collection.InsertOne(doc);
    }
}
