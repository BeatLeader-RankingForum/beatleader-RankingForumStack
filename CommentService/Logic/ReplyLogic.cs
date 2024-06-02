using CommentService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Logic;

public class ReplyLogic
{
    private readonly CommentDbContext _dbContext;
    
    public ReplyLogic(CommentDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
}