﻿using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Backend.WebApi.Swagger;

/// <summary>
/// Coneverts ann tags of OAS document to lowercase.
/// </summary>
/// <remarks>
/// In both types of places in document tree: tags in OpenaAPI Object and Paths->Operation Item->Tags.
/// </remarks>
public class LowerCaseTagsDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var pathTags = swaggerDoc.Paths.SelectMany(p => p.Value.Operations.SelectMany(o => o.Value.Tags));
        TagsToLower(pathTags.ToList());
        TagsToLower(swaggerDoc.Tags.ToList());
    }
    static void TagsToLower(List<Microsoft.OpenApi.Models.OpenApiTag> list)
    {
        list.ForEach(tag => tag.Name = tag.Name.ToLower());
    }
}