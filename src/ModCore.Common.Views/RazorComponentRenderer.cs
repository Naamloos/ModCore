using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using ModCore.Common.Discord.Entities.Components;

namespace ModCore.Common.Views
{
    public class RazorComponentRenderer
    {
        private readonly IRazorViewEngine _engine;
        private readonly ITempDataProvider _tempData;
        private readonly IServiceProvider _services;

        public RazorComponentRenderer(IRazorViewEngine engine, ITempDataProvider tempData, IServiceProvider services)
        {
            _engine = engine;
            _tempData = tempData;
            _services = services;
        }

        public async Task<IReadOnlyList<Component>> RenderAsync(string viewPath, object? model = null)
        {
            using var sw = new StringWriter();
            var httpCtx = new DefaultHttpContext { RequestServices = _services };

            // provide a place for TagHelpers to stash the builder
            var discordCtx = new RazorBuildContext();
            httpCtx.Items[nameof(RazorBuildContext)] = discordCtx;

            var actionCtx = new ActionContext(httpCtx, new RouteData(), new ActionDescriptor());
            var viewResult = _engine.GetView(executingFilePath: null, viewPath: viewPath, isMainPage: true);
            if (!viewResult.Success) throw new InvalidOperationException($"View '{viewPath}' not found.");

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model };
            var tempData = new TempDataDictionary(httpCtx, _tempData);

            var viewCtx = new ViewContext(actionCtx, viewResult.View, viewData, tempData, sw, new HtmlHelperOptions());

            await viewResult.View.RenderAsync(viewCtx);

            // Ignore HTML; pull the payload your helpers constructed
            return discordCtx.ComponentStack;
        }
    }
}
