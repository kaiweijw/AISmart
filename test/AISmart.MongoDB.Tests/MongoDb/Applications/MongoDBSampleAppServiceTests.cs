using AISmart.MongoDB;
using AISmart.Samples;
using Xunit;

namespace AISmart.MongoDb.Applications;

[Collection(AISmartTestConsts.CollectionDefinitionName)]
public class MongoDBSampleAppServiceTests : SampleAppServiceTests<AISmartMongoDbTestModule>
{

}
