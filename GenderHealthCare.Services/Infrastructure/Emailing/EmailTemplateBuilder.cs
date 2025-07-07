using GenderHealthCare.Contract.Services.Interfaces;
using RazorLight;

namespace GenderHealthCare.Services.Infrastructure.Emailing
{
    public class EmailTemplateBuilder : IEmailTemplateBuilder
    {
        private readonly RazorLightEngine _engine;

        public EmailTemplateBuilder()
        {
            _engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(
                    assembly: typeof(EmailTemplateBuilder).Assembly,
                    rootNamespace: "GenderHealthCare.Services.Infrastructure.Templates"
                )
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> BuildAsync<T>(string templateName, T model)
        {
            var templateKey = $"Emails.CycleTracking.{templateName}.cshtml";
            return await _engine.CompileRenderAsync(templateKey, model);
        }
    }

}
