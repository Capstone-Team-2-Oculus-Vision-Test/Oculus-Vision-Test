# VR Visual Field Test
This project aims to create a visual field static perimetry test using Oculus Quest VR headsets as a cheaper and more widely available alternative to specialized machines like the Humphrey Visual Field Analyzer. It seeks to address current issues with VR implementations such as high costs and proprietary hardware. This project is created by our CSCE capstone group, in collaboration with one of the BMEG capstone groups.

## Overview
The VR Visual Field Test is a virtual reality application that allows users to perform a visual field test in a virtual environment. The application uses the Oculus Rift headset to create a 3D environment in which users can interact with. The visual field test is used to assess a patient’s peripheral vision and detect any blind spots or other visual field defects.

## Features
- Display central fixation point
- Emulate light flashes
- Record responses to stimuli
- Customizable test testings
- Eye tracking framework for error detection
- 20-2 and 30-2 standards

## Install Instructions
All required dependencies are packaged with the Github distribution.  The executable is built for Windows systems using Unity version 2021.3.17f1.  Building on your own will require installing the Unity Editor (https://unity.com/download)

  ### Dependencies
    "com.unity.inputsystem": "1.4.4",
    "com.unity.textmeshpro": "3.0.6",
    "com.unity.xr.oculus": "3.2.3",
    "com.unity.modules.tilemap": "1.0.0",
    "com.unity.modules.ui": "1.0.0",
    "com.unity.modules.uielements": "1.0.0",
    "com.unity.modules.vr": "1.0.0",
    "com.unity.modules.xr": "1.0.0"
    
  ### External Prerequisites
    - Oculus desktop app (https://www.meta.com/quest/setup/?utm_source=www.meta.com&utm_medium=dollyredirect)
    - Link cable between headset and PC (alternatively use Oculus air link)
    - Minimum required headset: Oculus Quest 2 (no eyetracking) or Oculus Quest Pro (eyetracking)
    
## Run Instructions
 1. Connect the Oculus headset to pc and pair through the Oculus desktop application
 2. On the headset, launch Quest Link through either a link cable or air link
 3. Run the executable on the desktop
 4. The desktop application handles the practitioner side of testing, you can set test settings as well as start a test
 5. The headset handles the patient side of testing, they press the 'A' button on the right-hand controller to respond to stimuli

## Practitioner Instructions
1. Make adjustments to test setting and enter patient information as necessary
2. Press start test
3. You will see a live view of points being tested, values in decibels will fill in as the patient responds to them.  You may pause or restart the test at any time.
4. At the end of the test, you will be presented with the results and have the option to export them to pdf format and/or write them to the database

## Patient Instructions (to be given as a sort of tutorial)
1. You can adjust the brightness of points for the purpose of a short a tutorial to make sure the patient understands the test.
2. The patient is to simply look with one eye at a time at the central fixation point, and press the 'A' button whenever they notice a light flash.
3. Walk them through part of a test with the brightness turned up, and have them respond to a few points.  Make sure they are not pressing the button in a rhythm or simply at random.
