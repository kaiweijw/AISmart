using System;
using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace AISmart.MongoDB;

[DependsOn(
    typeof(AISmartApplicationTestModule),
    typeof(AISmartMongoDbModule)
)]
public class AISmartMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = AISmartMongoDbFixture.GetRandomConnectionString();
        });
    }
}
