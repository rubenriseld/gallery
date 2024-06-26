﻿using Gallery.API.DTOs;

namespace Gallery.API.Interfaces;

public interface ITagService
{
    Task<List<ReadTagDTO>> GetAllTags();
    Task<ReadTagDTO> CreateTag(CreateTagDTO createTagDto);
    Task<ReadTagDTO> UpdateTag(string tagId, UpdateTagDTO updateTagDto);
    Task DeleteTag(string tagId);
}