using Core.Interfaces.Helper;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Data.Dto;

namespace Core.Services
{
    public class ConfirmAssistanceService : IConfirmAssistanceService
    {
        private readonly IFamilyRepository _familyRepository;
        private readonly IEmailHelper _emailHelper;
        private readonly IPdfHelper _pdfHelper;

        public ConfirmAssistanceService(IFamilyRepository familyRepository, IEmailHelper emailHelper, IPdfHelper pdfHelper)
        {
            _familyRepository = familyRepository;
            _emailHelper = emailHelper;
            _pdfHelper = pdfHelper;
        }

        public async Task ConfirmAssistanceAsync(string eMailTo, string invitationCode)
        {
            FamilyDto familyDto = await _familyRepository.GetOneByInvitationCodeAsync(invitationCode);

            if (familyDto == null)
                throw new NullReferenceException("No existe información con ese código de Invitación");


           
            string generatePdfFile = _pdfHelper.MakePDF(invitationCode, familyDto.LastName, familyDto.FamilyMembers.Select(s => s.Names).ToList());
            string generatedJpgImage = await _pdfHelper.ConvertPdfToImage(generatePdfFile, invitationCode);

            await _emailHelper.SendEmailAsync(new MailRequestDto(

                eMailTo,
                "Nuestra Boda - Mayra & Carlos",
                "<html><body><img src=\"cid:image1\"></body></html>",
                generatedJpgImage
            ));

            if(familyDto.ConfirmationDate == null)
            {
                _familyRepository.Update(new FamilyDto()
                {
                    Id = familyDto.Id,
                    InvitationCode = invitationCode,
                    LastName = familyDto.LastName,
                    EmailAddress = eMailTo,
                    ConfirmationDate = DateTime.UtcNow
                });
            }
        }
    }
}
