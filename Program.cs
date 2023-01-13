using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace dotnet.echo
{
  public class Echo
  {

    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);
      var app = builder.Build();
      app.Use((context, next) =>
      {
        context.Request.EnableBuffering();
        return next();
      });
      app.Run(async context =>
      {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($":method: {context.Request.Method}");
        sb.AppendLine($":url: {context.Request.Path}");
        foreach (var header in context.Request.Headers)
        {
          sb.AppendLine($"{header.Key}: {header.Value}");
        }
        sb.AppendLine();

        var req = context.Request;
        req.EnableBuffering();
        if (req.Body.CanSeek)
        {
          req.Body.Seek(0, SeekOrigin.Begin);
          using (var reader = new StreamReader(
               req.Body,
               encoding: Encoding.UTF8,
               detectEncodingFromByteOrderMarks: false,
               bufferSize: 8192,
               leaveOpen: true))
          {
            var data = await reader.ReadToEndAsync();
            sb.AppendLine(data);
          }
          req.Body.Seek(0, SeekOrigin.Begin);
        }
        await context.Response.WriteAsync(sb.ToString());
      });
      app.Run();
    }
  }
}