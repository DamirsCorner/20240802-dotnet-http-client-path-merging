using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HttpClientPathMerging.Tests;

public class HttpClientTests
{
    private WebApplicationFactory<Program> factory;

    [OneTimeSetUp]
    public void Setup()
    {
        factory = new WebApplicationFactory<Program>();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        factory.Dispose();
    }

    [TestCase("base1/base2/", "request", "base1/base2/request")]
    [TestCase("base1/base2/", "/request", "request")]
    [TestCase("base1/base2", "request", "base1/request")]
    [TestCase("base1/base2", "/request", "request")]
    public async Task PathMerging(string basePath, string requestPath, string mergedPath)
    {
        using var httpClient = factory.CreateClient();
        var host = httpClient.BaseAddress;
        httpClient.BaseAddress = new Uri(host + basePath);

        var response = await httpClient.GetAsync(requestPath);

        response?.RequestMessage?.RequestUri.Should().Be(host + mergedPath);
    }
}
