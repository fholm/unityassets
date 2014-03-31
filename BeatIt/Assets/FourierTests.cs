using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Exocortex.DSP;

public class FourierTests : MonoBehaviour
{
    /*
    public class AudioSample
    {
        public float[] Left = new float[1024];
        public float[] Right = new float[1024];
        public float[] Energy = new float[1024];

        public void ComputeEnergy()
        {
            for(int i = 0; i < Left.Length; ++i)
            {
                Energy[i] = (Left[i] * Left[i]) + (Right[i] * Right[i]);
            }
        }
    }
    */

    const int subBandSize = 128;

    int h = 0;
    int count = 0;
    float[] _Ei_ = new float[subBandSize];
    float[][] Ei = new float[43][];
    float[] Es = new float[subBandSize];
    float[] module = new float[1024];
    float[] leftData = new float[1024];
    float[] rightData = new float[1024];
    ComplexF[] fftData = new ComplexF[1024];

    float lastSample = 0;

    [SerializeField]
    AudioClip clip;

    [SerializeField]
    Transform left;

    [SerializeField]
    Transform right;

    [SerializeField]
    AudioListener listener;

    [SerializeField]
    AudioClip[] clips = new AudioClip[0];

    GameObject[] subBands = new GameObject[subBandSize];
    Vector3[] scales = new Vector3[subBandSize];

    void Start()
    {
        audio.PlayOneShot(clips[2]);

        for (int i = 0; i < Ei.Length; ++i)
        {
            Ei[i] = new float[subBandSize];
        }

        float offset = -((subBands.Length * 0.15f) / 2f);

        for (int i = 0; i < subBands.Length; ++i)
        {
            subBands[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            subBands[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            subBands[i].transform.position = new Vector3(offset + 0.15f * i, 0, 10);
            scales[i] = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }

    void Update()
    {
        if (true) //Time.time - lastSample >= 0.043f)
        {

            // Set last sample time
            lastSample = Time.time;

            // Unity specific to pull out sample data, each array is filed with 1024 samples
            AudioListener.GetSpectrumData(leftData, 0, FFTWindow.BlackmanHarris);
            AudioListener.GetSpectrumData(rightData, 1, FFTWindow.BlackmanHarris);

            // Debug.Log(leftData[0]);

            /*
            // Copy sample data into real (left) and imaginary parts (right)
            for (int i = 0; i < leftData.Length; ++i)
            {
                fftData[i].Re = leftData[i];
                fftData[i].Im = rightData[i];
            }

            // Compute FFT on data (this FFT is correct, used it many times before)
            Fourier.FFT(fftData, FourierDirection.Forward);
            */

            /*
            for (int i = 256; i < fftData.Length; ++i)
            {
                fftData[i].Re = 0f;
                fftData[i].Im = 0f;
            }
            */

            // Compute the square module of FFT result
            for (int i = 0; i < fftData.Length; ++i)
            {
                // Real square
                float reSquare = fftData[i].Re * fftData[i].Re;

                // Imaginary square
                float imSquare = fftData[i].Im * fftData[i].Im;

                // Sum of squares + module
                module[i] = (imSquare + reSquare);

                // Meep
                module[i] = (leftData[i] * leftData[i]) + (rightData[i] * rightData[i]);
            }

            // Create a new energy array to store our 32 sub-bands in
            Es = new float[subBandSize];

            // Calculate samples per suband
            int samplesPerSubBand = fftData.Length / Es.Length;

            // Compute energy
            for (int i = 0; i < Es.Length; ++i)
            {
                Es[i] = 0;

                // This copies the output 0-31 into energy[0], 32-63 into energy[1], etc.
                for (int m = 0; m < samplesPerSubBand; ++m)
                {
                    Es[i] += module[(i * samplesPerSubBand) + m];
                }

                Es[i] /= samplesPerSubBand;
            }

            // This just shifts the previous energies array forward so we can set our new energy array at 0
            for (int i = (Ei.Length - 2); i >= 0; --i)
            {
                Ei[i + 1] = Ei[i];
            }

            // Write latest as first
            Ei[0] = Es;

            // Increase count by one
            count = Mathf.Min(count + 1, 43);

            // We need 43 samples to detect beats
            if (count == 43)
            {
                // Compute avarage energy of previous samples
                // this array is 32 wide (one for each subband)
                for (int i = 0; i < _Ei_.Length; ++i)
                {
                    // Clear current value from previous frame
                    _Ei_[i] = 0;

                    for (int n = 0; n < Ei.Length; ++n)
                    {
                        // Add it's energy for the sub-band i to the total
                        _Ei_[i] += Ei[n][i];
                    }

                    // We store 43 samples backwards
                    _Ei_[i] /= 43f;
                }

                for (int i = 0; i < Es.Length; ++i)
                {
                    // Debug.Log(string.Format("Es[{0}] = {1} and _Ei_[{0}] = {2}", i, Es[i], _Ei_[i]));
                    // Check if we have a beat by comparing Es[i] with 250 * _Ei_[i]
                    if (Es[i] > (4f * _Ei_[i]))
                    {
                        scales[i] = new Vector3(0.1f, 1f, 0.1f);
                    }
                    else
                    {
                        scales[i] = new Vector3(0.1f, 0.1f, 0.1f);
                    }
                }
            }
        }

        for (int i = 0; i < scales.Length; ++i)
        {
            Vector3 scale = subBands[i].transform.localScale;

            if (scales[i].y == 1f)
            {
                subBands[i].transform.localScale = scales[i];
            }
            else
            {
                subBands[i].transform.localScale = Vector3.Lerp(scale, scales[i], Time.deltaTime / 0.25f);
            }
        }
    }

    void Update2()
    {
        if (true) //Time.time - lastSample >= 0.043f)
        {

            // Set last sample time
            lastSample = Time.time;

            // Unity specific to pull out sample data, each array is filed with 1024 samples
            AudioListener.GetOutputData(leftData, 0);
            AudioListener.GetOutputData(rightData, 1);

            // Debug.Log(leftData[0]);

            // Copy sample data into real (left) and imaginary parts (right)
            for (int i = 0; i < leftData.Length; ++i)
            {
                fftData[i].Re = leftData[i];
                fftData[i].Im = rightData[i];
            }

            // Compute FFT on data (this FFT is correct, used it many times before)
            Fourier.FFT(fftData, FourierDirection.Forward);

            /*
            for (int i = 256; i < fftData.Length; ++i)
            {
                fftData[i].Re = 0f;
                fftData[i].Im = 0f;
            }
            */

            // Compute the square module of FFT result
            for (int i = 0; i < fftData.Length; ++i)
            {
                // Real square
                float reSquare = fftData[i].Re * fftData[i].Re;

                // Imaginary square
                float imSquare = fftData[i].Im * fftData[i].Im;

                // Sum of squares + module
                module[i] = (imSquare + reSquare);
            }

            // Create a new energy array to store our 32 sub-bands in
            Es = new float[32];

            // Calculate samples per suband
            int samplesPerSubBand = fftData.Length / Es.Length;

            // Compute energy
            for (int i = 0; i < Es.Length; ++i)
            {
                Es[i] = 0;

                // This copies the output 0-31 into energy[0], 32-63 into energy[1], etc.
                for (int m = 0; m < samplesPerSubBand; ++m)
                {
                    Es[i] += module[(i * samplesPerSubBand) + m];
                }

                Es[i] /= 32f;
            }

            // This just shifts the previous energies array forward so we can set our new energy array at 0
            for (int i = (Ei.Length - 2); i >= 0; --i)
            {
                Ei[i + 1] = Ei[i];
            }

            // Copy new data into history slot h
            // System.Buffer.BlockCopy(Es, 0, Ei[h], 0, Es.Length);

            // Move index forward
            // h = (h + 1) % 43;

            // Write latest as first
            Ei[0] = Es;

            // Increase count by one
            count = Mathf.Min(count + 1, 43);

            // We need 43 samples to detect beats
            if (count == 43)
            {
                // Compute avarage energy of previous samples
                // this array is 32 wide (one for each subband)
                for (int i = 0; i < _Ei_.Length; ++i)
                {
                    // Clear current value from previous frame
                    _Ei_[i] = 0;

                    for (int n = 0; n < Ei.Length; ++n)
                    {
                        // Add it's energy for the sub-band i to the total
                        _Ei_[i] += Ei[n][i];
                    }

                    // We store 43 samples backwards
                    _Ei_[i] /= 43f;
                }

                for (int i = 0; i < Es.Length; ++i)
                {
                    // Debug.Log(string.Format("Es[{0}] = {1} and _Ei_[{0}] = {2}", i, Es[i], _Ei_[i]));
                    // Check if we have a beat by comparing Es[i] with 250 * _Ei_[i]
                    if (Es[i] > (2.5f * _Ei_[i]))
                    {
                        scales[i] = new Vector3(0.1f, 1f, 0.1f);
                    }
                    else
                    {
                        scales[i] = new Vector3(0.1f, 0.1f, 0.1f);
                    }
                }
            }
        }

        for (int i = 0; i < scales.Length; ++i)
        {
            Vector3 scale = subBands[i].transform.localScale;

            if (scales[i].y == 1f)
            {
                subBands[i].transform.localScale = scales[i];
            }
            else
            {
                subBands[i].transform.localScale = Vector3.Lerp(scale, scales[i], Time.deltaTime / 0.25f);
            }
        }
    }

    void OnGUI()
    {
        for (int i = 0; i < clips.Length; ++i)
        {
            if (GUILayout.Button(clips[i].name))
            {
                count = 0;
                audio.Stop();
                audio.PlayOneShot(clips[i]);
            }
        }
    }
}
