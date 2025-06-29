﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsPaper.src.Application.DTOs;
using NewsPaper.src.Application.Features;
using NewsPaper.src.Application.Services;
using NewsPaper.src.Domain.Interfaces;

namespace NewsPaper.src.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : BaseController<CommentController>
    {
        private readonly CommentService _commentService;
        private readonly ILogger<CommentController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CommentController(CommentService commentService, ILogger<CommentController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _commentService = commentService;
        }

        [HttpPost("CreateNewComment")]
        public async Task<ResponseData> CreateNewComment(CommentDto newComment)
        {
            var comment = await _commentService.CreateNewComment(newComment, UserIDLogined);
            return new ResponseData { Data = comment, StatusCode = 1 };
        }

        [HttpGet("GetListCommentByNews")]
        [AllowAnonymous]
        public async Task<ResponseData> GetListCommentByNews(int newsID)
        {
            try
            {
                var comment = await _commentService.GetCommentInPost(newsID);
                return new ResponseData { Data = comment, StatusCode = 1 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for news {NewsId}", newsID);
                return new ResponseData { Data = new List<object>(), StatusCode = 1 };
            }
        }

        [HttpGet("GetAllCommentAdmin")]
        [Authorize(Roles = "1")]
        public async Task<ResponseData> GetAllCommentAdmin()
        {
            var comment = await _commentService.GetAllCommentAdmin();
            return new ResponseData { Data = comment, StatusCode = 1 };
        }

        [HttpDelete("DeleteComment")]
        public async Task<ResponseData> DeleteComment(int commentID)
        {
            var comment = await _commentService.DeleteComment(commentID, UserIDLogined);
            return new ResponseData { Data = comment, StatusCode = 1 };
        }

        [HttpDelete("DeleteCommentByAdmin")]
        [Authorize(Roles = "1")]
        public async Task<ResponseData> DeleteCommentByAdmin(int commentID)
        {
            var comment = await _commentService.DeleteCommentByAdmin(commentID);
            return new ResponseData { Data = comment, StatusCode = 1 };
        }
    }
}