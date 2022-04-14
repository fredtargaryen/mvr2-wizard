# mvr2-wizard
Interactive Unity wizard for installing MiddleVR 2 quickly, on URP, HDRP and the built-in render pipeline. MiddleVR packages not included.

## Installation, Use and Uninstalling
The wizard can be installed via the classic method of right-clicking an empty folder space in the Project panel and selecting `Import Package > Custom Package...`. You can then open the wizard window by selecting `MiddleVR > Setup Wizard` from the Menu bar. When you are finished using it, the wizard will cheerfully send itself to the Recycle Bin if `Remove Wizard From Project` is clicked.

## Customisation
This generic wizard will only get you as far as installing the necessary packages and prefabs to set up MiddleVR in your Unity project. You will need to customise it further to support your specific MiddleVR configuration.

If you want to make the wizard add extra GameObjects to support a custom Scriptable Render Pipeline, it's best to add a new case underneath the `else if(foundMVRURP)` block in `MiddleVRInstallHelper.cs` and copy the patterns in the above blocks of code.

If you want the wizard to take the user through setting up custom configurations, edit `DrawCustomWizardOptions()` in `MiddleVRInstallHelper.cs`.

## Supported Unity versions
This project should support all Unity versions supported by MiddleVR 2.

## Contribution
Apart from being a speedy way to install MiddleVR 2 without the tutorial, this project is intended as a base to be modified for specific MiddleVR use cases. Merge requests will only be accepted if they benefit all or the overwhelming majority of use cases.