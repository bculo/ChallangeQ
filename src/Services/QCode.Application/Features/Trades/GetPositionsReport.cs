using FluentValidation;
using MediatR;
using MimeTypes;
using QCode.Core.Exceptions;
using System.IO.Abstractions;

namespace QCode.Application.Features.Trades
{
    public static class GetPositionsReport
    {
        public class Query : IRequest<Response> 
        {
            public string? FullFilePath { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.FullFilePath).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IFileSystem _fileSystem;

            public Handler(IFileSystem fileSystem)
            {
                _fileSystem = fileSystem;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                if(!_fileSystem.File.Exists(request.FullFilePath))
                {
                    throw new QCodeNotFoundException($"File not found on path {request.FullFilePath}");
                }

                var bytes = await File.ReadAllBytesAsync(request.FullFilePath);

                return new Response
                {
                    FileName = _fileSystem.Path.GetFileName(request.FullFilePath),
                    ContentType = MimeTypeMap.GetMimeType(_fileSystem.Path.GetExtension(request.FullFilePath)),
                    Base64Content = Convert.ToBase64String(bytes),
                    Extension = _fileSystem.Path.GetExtension(request.FullFilePath)
                };
            }
        }

        public class Response
        {
            public string? Extension { get; set; }
            public string? FileName { get; set; }
            public string? ContentType { get; set; }
            public string? Base64Content { get; set; }
        }
    }
}
