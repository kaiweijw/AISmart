using AISmart.Samples;
using Xunit;

namespace AISmart.MongoDB.Domains;

[Collection(AISmartTestConsts.CollectionDefinitionName)]
public class MongoDBSampleDomainTests : SampleDomainTests<AISmartMongoDbTestModule>
{

}
