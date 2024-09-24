// See https://aka.ms/new-console-template for more informat

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

Console.WriteLine("Hello, 大象!");

// 定义冰箱
var bingxiang = new ServiceCollection();
// 装入大象
bingxiang.AddScoped<IElephant,RedElephant>();
bingxiang.AddScoped<IElephant,BlueElephant>();
bingxiang.AddScoped<IElephant,GreenElephant>();

bingxiang.AddScoped<RedElephant>();
bingxiang.AddSingleton<BlueElephant>();
bingxiang.AddTransient<GreenElephant>();



// 放入馒头
bingxiang.AddTransient<Mantou>();
bingxiang.AddKeyedTransient<Mantou>("绿馒头", (a, b) => {
    return new Mantou { Name = "绿馒头" };
});
bingxiang.AddTransient(a=>new Mantou { Name="黄色馒头"});


// 配置
var configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json",true,true).Build();
//bingxiang.Configure<SystemSetting>(configuration);
bingxiang.Configure<SystemSetting>(configuration.GetSection(null!));
bingxiang.Configure<SystemSetting>(op =>
{
    op.Desc = op.Desc+1;
});

// 关门
var BigBingxiang = bingxiang.BuildServiceProvider();

// 拿出所有
var qun = BigBingxiang.GetRequiredService<IEnumerable<IElephant>>();
// 拿出来单个
var red = BigBingxiang.GetRequiredService<RedElephant>();
var blue = BigBingxiang.GetRequiredService<BlueElephant>();
var green = BigBingxiang.GetRequiredService<GreenElephant>();

// 进入平行宇宙
using (var scope = BigBingxiang.CreateScope())
{
    // 平行宇宙拿出
    var red2 = scope.ServiceProvider.GetRequiredService<RedElephant>();
    // 超越平行宇宙的唯一
    var blue2 = scope.ServiceProvider.GetRequiredService<BlueElephant>();
    // 鱼的记忆
    var green2 = scope.ServiceProvider.GetRequiredService<GreenElephant>();
}

// 同一宇宙
var red3 = BigBingxiang.GetRequiredService<RedElephant>();
// 鱼的记忆
var green3 = BigBingxiang.GetRequiredService<GreenElephant>();

qun.ToList().ForEach(x => (x as IElephant).PutIn());
(red as IElephant).PutIn();
(blue as IElephant).PutIn();
(green as IElephant).PutIn();

var op = BigBingxiang.GetRequiredService<IOptionsMonitor<SystemSetting>>();
op.OnChange(x => {
    Console.WriteLine(x.Desc);
});

Console.WriteLine("end");

while (true)
{
    Thread.Sleep(3000);
}


interface IElephant
{
    string Name { get; }
    public void PutIn()
    {
        Console.WriteLine($"{Name}被放入");
    }
}

class RedElephant:IElephant
{
    private readonly Mantou _mantou;
    public RedElephant(Mantou mantou)
    {
        _mantou = mantou;
    }
    public string Name => "红象";
}
class BlueElephant : IElephant
{
    public string Name => "蓝象";
}
class GreenElephant : IElephant
{
    Mantou _mantou;
    public GreenElephant([FromKeyedServices("绿馒头")]Mantou mantou)
    {
        _mantou = mantou;
    }

    public string Name => "绿象";
}

class Mantou
{
    public string Name { get; set; } = "红馒头";
}

class SystemSetting
{
    public string Desc { get; set; } = default!;
}