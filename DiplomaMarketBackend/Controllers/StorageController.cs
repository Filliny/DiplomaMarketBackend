using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaMarketBackend.Controllers;

public class StorageController:ControllerBase
{
    ILogger<GoodsController> _logger;
    BaseContext _context;
    IFileService _fileService;

    public StorageController(ILogger<GoodsController> logger, BaseContext context, IFileService fileService)
    {
        _logger = logger;
        _context = context;
        _fileService = fileService;
    }
    
    
}