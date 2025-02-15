/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

//This file is deprecated.  Use the high level voip system instead:
// https://developer.oculus.com/documentation/unity/ps-voip/ 

#if OVR_PLATFORM_USE_MICROPHONE
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Oculus.Platform
{
  public class MicrophoneInputNative : IMicrophone
  {
    IntPtr mic;

    int tempBufferSize = 960 * 10;
    float[] tempBuffer;

    private Dictionary<int, float[]> micSampleBuffers;

    public MicrophoneInputNative()
    {
      mic = CAPI.ovr_Microphone_Create();
      CAPI.ovr_Microphone_Start(mic);
      tempBuffer = new float[tempBufferSize];

      micSampleBuffers = new Dictionary<int, float[]>();
    }

    public float[] Update()
    {
      ulong readSize = (ulong)CAPI.ovr_Microphone_ReadData(mic, tempBuffer, (UIntPtr)tempBufferSize);
      if (readSize > 0)
      {
        float[] samples;
        if (!micSampleBuffers.TryGetValue((int)readSize, out samples))
        {
          samples = new float[readSize];
          micSampleBuffers[(int)readSize] = samples;
        }
        Array.Copy(tempBuffer, samples, (int)readSize);
        return samples;
      }
      return null;
    }

    public void Start()
    {

    }

    public void Stop()
    {
      CAPI.ovr_Microphone_Stop(mic);
      CAPI.ovr_Microphone_Destroy(mic);
    }
  }
}
#endif
