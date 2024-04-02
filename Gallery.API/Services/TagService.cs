using AutoMapper;
using Gallery.API.DTOs;
using Gallery.API.Interfaces;
using Gallery.Database.Entities;
using Gallery.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gallery.API.Services;

public class TagService : ITagService
{
    private readonly IRepository<Tag> _tagRepository;
    private readonly IMapper _mapper;


    public TagService(
        IRepository<Tag> tagRepository, IMapper mapper) =>
        (_tagRepository, _mapper) = (tagRepository, mapper);

    public async Task<ReadTagDTO> CreateTag(CreateTagDTO createTagDto)
    {
        var tagEntity = _mapper.Map<Tag>(createTagDto);
        await _tagRepository.Add(tagEntity);
        await _tagRepository.SaveChanges();

        return _mapper.Map<ReadTagDTO>(tagEntity);
    }
    public async Task<List<ReadTagDTO>> GetAllTags()
    {
        var tags = await _tagRepository.Get().ToListAsync();

        return _mapper.Map<List<ReadTagDTO>>(tags);
    }

    public async Task<ReadTagDTO> UpdateTag(UpdateTagDTO updateTagDto)
    {
        var tag = await _tagRepository.Find(updateTagDto.TagId);

        _mapper.Map(updateTagDto, tag);
        _tagRepository.Update(tag);
        await _tagRepository.SaveChanges();
        return _mapper.Map<ReadTagDTO>(tag);
    }
    public async Task DeleteTag(string tagId)
    {
        var tag = await _tagRepository.Find(tagId);
        _tagRepository.Delete(tag);
        await _tagRepository.SaveChanges();
    }
}
