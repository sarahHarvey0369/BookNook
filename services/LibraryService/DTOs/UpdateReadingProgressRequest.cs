using System.ComponentModel.DataAnnotations;
using BookNook.LibraryService.Models;

namespace BookNook.LibraryService.DTOs;

public class UpdateReadingProgressRequest
{
    [EnumDataType(typeof(ReadingStatus))]
    public ReadingStatus Status { get; set; }

    [Range(0, int.MaxValue)]
    public int? CurrentPage { get; set; }
}