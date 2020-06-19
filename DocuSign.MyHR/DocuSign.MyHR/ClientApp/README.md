# ASP.Net Core 3.1 and Angular 9: MyHR Sample Application

### GitHub repo: sample-app-myhr-csharp
Visit [MyHR sample application](https://myhrsample.esigndemos.com/) on DocuSign to see this code publicly hosted

## Introduction
MyHRsample application written in ASP.Net Core 3.1 (server) and Angular 9 (client). You can find a live instance running at [https://myhrsample.esigndemos.com/](https://myhrsample.esigndemos.com/).

MyHR demonstrates the following:
1. Authentication with two different methods
    * [Auth Code Grant](https://developers.docusign.com/esign-rest-api/guides/authentication/oauth2-code-grant)
    * [JSON Web Token (JWT) Grant](https://developers.docusign.com/esign-rest-api/guides/authentication/oauth2-jsonwebtoken)

2. User information is shown from the DocuSign account. This example demonstrates  [Users API](https://developers.docusign.com/esign-rest-api/guides/authentication/user-info-endpoints) functionality.

3. Direct Deposit update. This example demonstrates filling in bank account information for direct deposit and submitting it to be processed by payroll.

4. W-4 Tax withholding. This example demonstrates filling in a standard W-4 form required by IRS from all US employees.

5. Time tracking. This example shows how to use the Click API to create a clickwrap programmatically, render it in your UI, and then submit it. It also tracks the submission event and, just after submission, redirects the user back to the start page.  
   * [More information about Click API](https://developers.docusign.com/click-api)
6.	Tuition Reimbursement. This example demonstrates submitting a request for reimbursement. To prove that a class was completed users can also attach some written proof before submitting their request for reimbursement. Adding attachments lets users add additional information to the document.
     * [More information about Adding attachments](https://support.docusign.com/en/guides/signer-guide-signing-adding-attachments-new)
7. Send an offer letter to candidate. This example demonstrates sending the offder letter to the individuals in the organization. The offer is approved internally first by the user and then sent to a candidate for a sigh.

8. Send an I-9 verification Request to candidate using IDV. This example demonstrates sending the Federal I-9 to a new hire.
   * [More information about ID Verification](https://developers.docusign.com/esign-rest-api/guides/concepts/recipient-authentication#id-verification-idv)

The examples with templates were created using next DocuSign APIs and Features:
   * The Docusign [Template API](https://developers.docusign.com/esign-rest-api/code-examples/code-example-create-template) functionality.
   * The signing ceremony is implemented with embedded signing for a single signer.
   * The DocuSign signing ceremony is initiated from your website.  
   * Anchor text ([AutoPlace](https://support.docusign.com/en/guides/AutoPlace-New-DocuSign-Experience)) is used to position the signing fields in the document.

## Installation

### Prerequisites
* A DocuSign Developer Sandbox account (email and password) on [demo.docusign.net](https://demo.docusign.net). If you don't already have a developer sandbox, create a [free account](https://go.docusign.com/sandbox/productshot/?elqCampaignId=16535).
* A DocuSign integration key (a client ID) that is configured to use **JSON Web Token (JWT) Grant**.
   You will need the **integration key** itself and its **RSA key pair**. To use this application, you must add your application's **Redirect URI** to your integration key. This [**video**](https://www.youtube.com/watch?v=GgDqa7-L0yo) demonstrates how to create an integration key (client ID) for a user application such as this example.
* C# .NET Core version 3.1 or later.
* [Node.js](https://nodejs.org/) v10+

### Installation steps
**Manual**
1. Download or clone this repository to your workstation in a new folder named **sample-app-myhr-csharp**.
2. The repository includes a Visual Studio 2019 solution file and NuGet package references in the project file.
3. Modify the appsettings.json (the configuration file) with file with the integration key and other settings from your DocuSign Developer Sandbox account.
    > **Note:** Protect your integration key and client secret. You should make sure that the **.env** file will not be stored in your source code repository.
4. Navigate to that folder: cd sample-app-myhr-csharp
5. Install client side dependencies using the npm package manager: npm install

**Using installation scripts**

## Running MyHR
**Manual**
1. Build and then start the solution.
2. Your default browser will be opened to https://localhost:5001/ and you will see the application's home page.

**Using installation scripts**

## License information
This repository uses the MIT License. See the [LICENSE](./LICENSE) file for more information.