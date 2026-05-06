using Elaaj.Application.Interfaces;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.Features.Posts.Commands.CreatePost;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, int>
{
    private readonly IGenericRepository<Post> _repository;
    private readonly IFileService _fileService;

    public CreatePostCommandHandler(IGenericRepository<Post> repository, IFileService fileService)
    {
        _repository = repository;
        _fileService = fileService;
    }

    public async Task<int> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        string? imageUrl = null;
        if (request.File != null && request.File.Length > 0)
        {
            imageUrl = await _fileService.UploadFileAsync(request.File, "generalposts");
        }

        var Post = new Post
        {
            UserId = request.UserId,
            Content = request.Content,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(Post);
        await _repository.SaveChangesAsync();

        return Post.Id;
    }
}