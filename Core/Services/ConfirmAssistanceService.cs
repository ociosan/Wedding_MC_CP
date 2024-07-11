using Core.Enum;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Interfaces.UnitOfWork;
using Data.Dto;

namespace Core.Services
{
    public class ConfirmAssistanceService : IConfirmAssistanceService
    {
        private readonly IFamilyRepository _familyRepository;
        private readonly IHelpersUow _helpersUow;
        private readonly IAzureUow _azureUow;

        public ConfirmAssistanceService(
            IFamilyRepository familyRepository,
            IHelpersUow helpersUow,
            IAzureUow azureUow)
        {
            _familyRepository = familyRepository;
            _helpersUow = helpersUow;
            _azureUow = azureUow;
        }

        public async Task ConfirmAssistanceAsync(ConfirmAssitanceDto confirmAssitanceDto)
        {

            FamilyDto familyDto = await _familyRepository.GetOneByInvitationCodeAsync(confirmAssitanceDto.InvitationCode);

            if (familyDto == null)
                throw new NullReferenceException("No existe información con ese código de Invitación");

            await _helpersUow.Pdf.MakePDF(
                confirmAssitanceDto.InvitationCode,
                familyDto.LastName,
                familyDto.FamilyMembers.Select(s => s.Names).ToList(),
                await _azureUow.StorageAccount.DownloadInvitationTemplateAsync());

            await _helpersUow.Email.SendEmailAsync(new MailRequestDto(

                confirmAssitanceDto.Email,
                "Nuestra Boda - Mayra & Carlos",
                "<html><body><img src=\"cid:image1\"></body></html>",
                await _helpersUow.Pdf.ConvertPdfToImage(confirmAssitanceDto.InvitationCode)
            ));

            await _helpersUow.Messages.SendWhatsappAsync(confirmAssitanceDto);

            if(familyDto.ConfirmationDate == null)
            {
                _familyRepository.Update(new FamilyDto()
                {
                    Id = familyDto.Id,
                    InvitationCode = confirmAssitanceDto.InvitationCode,
                    LastName = familyDto.LastName,
                    EmailAddress = confirmAssitanceDto.Email,
                    PhoneNumber = confirmAssitanceDto.PhoneNumber,
                    ConfirmationDate = DateTime.UtcNow
                });
            }
        }

        public async Task ReSendEmailAsync(ConfirmAssitanceDto confirmAssitanceDto)
        {
            if (!await _azureUow.StorageAccount.FileExistsAsync(confirmAssitanceDto.InvitationCode, FileTypeEnum.Jpg))
            {
                await ConfirmAssistanceAsync(confirmAssitanceDto);
                return;
            }

            await using Stream stream = new MemoryStream(await _azureUow.StorageAccount.DownloadInvitationAsync(confirmAssitanceDto.InvitationCode, FileTypeEnum.Jpg));
            /*await _helpersUow.Email.SendEmailAsync(new MailRequestDto(
                confirmAssitanceDto.Email,
                "Nuestra Boda - Mayra & Carlos",
                "<html><body><img src=\"cid:image1\"></body></html>",
                stream
                
            ));*/
        }
    }
}
