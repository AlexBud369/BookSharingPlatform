using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configuration;

public class SupabaseOptions
{
    [Required(ErrorMessage = "Supabase URL is required")]
    public string Url { get; set; } = string.Empty;

    [Required(ErrorMessage = "Supabase Public Key is required")]
    public string PublicKey { get; set; } = string.Empty;

    [Required(ErrorMessage = "Supabase Secret Key is required")]
    public string SecretKey { get; set; } = string.Empty;

    [Required(ErrorMessage = "Supabase Bucket Name is required")]
    public string BucketName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Supabase Public URL is required")]
    public string PublicUrl { get; set; } = string.Empty;
}
