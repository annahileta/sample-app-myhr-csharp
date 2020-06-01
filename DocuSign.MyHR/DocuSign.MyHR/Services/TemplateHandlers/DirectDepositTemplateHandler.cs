using DocuSign.eSign.Model;
using DocuSign.MyHR.Domain;
using System;
using System.Collections.Generic;
using System.IO;

namespace DocuSign.MyHR.Services.TemplateHandlers
{
    public class DirectDepositTemplateHandler : ITemplateHandler
    {
        private string _signerClientId = "1000";
        private string _templatePath = "/Templates/Direct Deposit.docx";

        public EnvelopeTemplate BuildTemplate(string rootDir)
        {
            var envelopeTemplate = new EnvelopeTemplate
            {
                Name = "Direct Deposit",
                EmailSubject = "Please sign this document",
                Documents = new List<Document> {
                    new Document
                    {
                        DocumentBase64 = Convert.ToBase64String(File.ReadAllBytes(rootDir + _templatePath)),
                        Name = "Direct Deposit Update",
                        FileExtension = "docx",
                        DocumentId = "1"
                    }
                },

                Recipients = new Recipients
                {
                    Signers = new List<Signer>
                    {
                        new Signer
                        {
                            RecipientId = "1",
                            RoleName = "Employee",
                            RoutingOrder = "1",
                            Tabs = CreateTabs()
                        }
                    }
                }
            };

            return envelopeTemplate;
        }

        public EnvelopeDefinition BuildEnvelope(UserDetails currentUser, UserDetails additionalUser)
        {
            var env = new EnvelopeDefinition
            {

                TemplateRoles = new List<TemplateRole>
                {
                    new TemplateRole
                    {
                        Email = currentUser.Email,
                        Name = currentUser.Name,
                        ClientUserId = _signerClientId,
                        RoleName = "Employee"
                    }
                },
                Status = "sent"
            };

            return env;
        }

        private Tabs CreateTabs()
        {
            Tabs signer1Tabs = new Tabs
            {
                SignHereTabs = new List<SignHere>
                {
                    new SignHere
                    {
                        XPosition = "170",
                        YPosition = "418",
                        Optional = "false",
                        StampType = "signature",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        Name = "SignHere",
                    }
                },
                DateSignedTabs = new List<DateSigned>
                {
                    new DateSigned
                    {
                        Name = "DateSigned",
                        DocumentId = "1",
                        PageNumber = "1",
                        XPosition = "419",
                        YPosition = "426",
                        TemplateLocked = "false",
                        TemplateRequired = "false",
                        TabType = "datesigned"
                    }
                },
                NumberTabs = new List<Number>
                {
                    new Number
                    {
                        TabLabel = "PrimaryRoutingNumber",
                        ValidationMessage = "Primary routing number is required",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        Required = "true",
                        MaxLength = "100",
                        XPosition = "374",
                        YPosition = "106",
                        Width = "151",
                        Height = "23",
                    },
                    new Number
                    {
                        TabLabel = "PrimaryAccountNumber",
                        ValidationMessage = "Primary account number is required",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        Required = "true",
                        MaxLength = "50",
                        XPosition = "374",
                        YPosition = "137",
                        Width = "151",
                        Height = "23",
                    },
                    new Number
                    {
                        TabLabel = "PercentageFirstAccount",
                        ValidationPattern = "^[1-9][0-9]?$|^100$",
                        ValidationMessage = "Percentage must be between 1 and 100",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        Required = "false",
                        MaxLength = "3",
                        XPosition = "374",
                        YPosition = "171",
                        Width = "151",
                        Height = "23",
                    },
                    new Number
                    {
                        TabLabel = "SecondaryRoutingNumber",
                        ValidationMessage = "Secondary routing number is required",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        Required = "false",
                        MaxLength = "50",
                        XPosition = "374",
                        YPosition = "204",
                        Width = "151",
                        Height = "23",
                    },
                    new Number
                    {
                        TabLabel = "SecondaryAccountNumber",
                        ValidationMessage = "Secondary account number is required",
                        Required = "false",
                        MaxLength = "50",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        XPosition = "374",
                        YPosition = "237",
                        Width = "151",
                        Height = "23",
                    },
                    new Number
                    {
                        TabLabel = "PercentageSecondAccount",
                        ValidationPattern = "^[1-9][0-9]?$|^100$",
                        ValidationMessage = "Percentage must be between 1 and 100",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        Required = "false",
                        MaxLength = "3",
                        XPosition = "374",
                        YPosition = "275",
                        Width = "151",
                        Height = "23",
                    }
                },
                NoteTabs = new List<Note>
                {
                    new Note
                    {
                        Value = "Total Percentage cannot be more than hundred",
                        Name = "Note to recipient",
                        TabLabel = "ValidationMessage",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        XPosition = "85",
                        YPosition = "370",
                        Width = "300",
                        Height = "20",
                        FontColor = "BrightRed",
                        ConditionalParentLabel = "NumberValidation",
                        ConditionalParentValue = "0"
                    }
                },
                FormulaTabs = new List<FormulaTab>
                { 
                    new FormulaTab
                    {
                        Formula = "[PercentageFirstAccount] + [PercentageSecondAccount]",
                        Name = "Formula Tab",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        XPosition = "378",
                        YPosition = "322",
                        RoundDecimalPlaces = "0",
                        Locked = "true",
                        ConcealValueOnDocument = "true",
                        TabLabel = "TotalPercentage",
                        TabType = "formula",
                        Hidden =  "false",
                        Width =  "42",
                    },
                    new FormulaTab
                    {
                        Formula = "[PercentageFirstAccount] + [PercentageSecondAccount] = 100",
                        Name = "Formula Tab",
                        DocumentId = "1",
                        PageNumber = "1",
                        RecipientId = "1",
                        RoundDecimalPlaces = "0",
                        Locked = "true",
                        ConcealValueOnDocument = "true",
                        TabLabel = "NumberValidation",
                        TabType = "formula",
                    }
                },
            };
            return signer1Tabs;
        }
    }
}
