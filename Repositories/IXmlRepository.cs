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
        return docs
            .Select(d =>
            {
                var xml = d.GetValue("Xml").AsString;
                return XElement.Parse(xml);
            })
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
