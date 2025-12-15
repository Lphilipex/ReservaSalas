using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReservaSalas.Data.ReservaSalas.Models;
using ReservaSalas.Models;
using System.ComponentModel.DataAnnotations;

//esse e o projeto 

public class RegisterModel : PageModel
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly IUserStore<Usuario> _userStore; 
    private readonly IUserEmailStore<Usuario> _emailStore; 
    public RegisterModel(
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager,
        IUserStore<Usuario> userStore) 
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userStore = userStore;
       
        _emailStore = GetEmailStore();
    }


    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }
    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    


    public class InputModel
    {
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Sobrenome é obrigatório.")]
        [Display(Name = "Sobrenome")]
        public string Sobrenome { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Formato de telefone inválido.")]
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "O Sexo é obrigatório.")]
        [Display(Name = "Sexo")]
        public string Sexo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Senha é obrigatória.")]
        [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres de comprimento.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [Compare("Password", ErrorMessage = "A senha e a confirmação de senha não coincidem.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public async Task OnGetAsync(string returnUrl = null)
    {
        ReturnUrl = returnUrl;
       
        if (Input == null) Input = new InputModel();
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        
        if (ModelState.IsValid)
        {
           
            var user = CreateUser();

          
            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

           
            user.Nome = Input.Nome;
            user.Sobrenome = Input.Sobrenome;
            user.Telefone = Input.Telefone;
            user.Sexo = Input.Sexo;
            
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                
                await _userManager.AddToRoleAsync(user, "Usuario");

                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        
        return Page();
    }

   
    private Usuario CreateUser()
    {
        try
        {
            return Activator.CreateInstance<Usuario>();
        }
        catch
        {
            throw new InvalidOperationException($"Não foi possível criar uma instância de '{nameof(Usuario)}'.");
        }
    }

    
    private IUserEmailStore<Usuario> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("O UserManager precisa de um UserStore que suporte email.");
        }
        return (IUserEmailStore<Usuario>)_userStore;
    }
}