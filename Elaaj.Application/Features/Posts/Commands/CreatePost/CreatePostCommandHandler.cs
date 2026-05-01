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

    public CreatePostCommandHandler(IGenericRepository<Post> repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var Post = new Post
        {
            UserId = request.UserId,
            Content = request.Content,
            ImageUrl = request.ImageUrl,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(Post);
        await _repository.SaveChangesAsync();

        return Post.Id;
    }
}