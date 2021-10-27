using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SensorCalibration
{
    // Obtained from JSON file
    public List<float> masterRotationMatrix;
    public List<float> subordinateRotationMatrix;
    public Vector3 masterTranslationVector;
    public Vector3 subordinateTranslationVector;

    // Used as calibration values
    public System.Numerics.Quaternion masterRotationNumerics;
    public System.Numerics.Quaternion subordinateRotationTransposedNumerics;
    public System.Numerics.Vector3 masterTranslationVectorNumerics;
    public System.Numerics.Vector3 subordinateTranslationVectorNumerics;

    public static SensorCalibration CreateFromJSONFile(string inputFileName)
    {
        StreamReader sr = new StreamReader(Application.dataPath + "/Calibrations/" + inputFileName);

        SensorCalibration fromJson = new SensorCalibration();
        fromJson = JsonUtility.FromJson<SensorCalibration>(sr.ReadLine());

        fromJson.masterTranslationVectorNumerics = new System.Numerics.Vector3(fromJson.masterTranslationVector.x, fromJson.masterTranslationVector.y, fromJson.masterTranslationVector.z);
        fromJson.subordinateTranslationVectorNumerics = new System.Numerics.Vector3(fromJson.subordinateTranslationVector.x, fromJson.subordinateTranslationVector.y, fromJson.subordinateTranslationVector.z);

        Matrix4x4 masterRotationMatrix = new Matrix4x4();
        Matrix4x4 subordinateRotationMatrixTransposed = new Matrix4x4();

        for (int row = 0; row < 3; ++row)
        {
            for (int col = 0; col < 3; ++col)
            {
                masterRotationMatrix[row, col] = fromJson.masterRotationMatrix[row * 3 + col];
                subordinateRotationMatrixTransposed[row, col] = fromJson.subordinateRotationMatrix[col * 3 + row];
            }
        }

        masterRotationMatrix[3, 3] = 1f;
        subordinateRotationMatrixTransposed[3, 3] = 1f;

        Quaternion masterRotation = masterRotationMatrix.rotation;
        Quaternion subordinateRotationTransposed = subordinateRotationMatrixTransposed.rotation;

        fromJson.masterRotationNumerics = new System.Numerics.Quaternion(masterRotation.x, masterRotation.y, masterRotation.z, masterRotation.w);
        fromJson.subordinateRotationTransposedNumerics = new System.Numerics.Quaternion(subordinateRotationTransposed.x, subordinateRotationTransposed.y,
                                                                                        subordinateRotationTransposed.z, subordinateRotationTransposed.w);

        // Zero pitch
        fromJson.masterRotationNumerics.X = 0f;
        fromJson.subordinateRotationTransposedNumerics.X = 0f;

        // Normalize because pitch was modified
        fromJson.masterRotationNumerics = System.Numerics.Quaternion.Normalize(fromJson.masterRotationNumerics);
        fromJson.subordinateRotationTransposedNumerics = System.Numerics.Quaternion.Normalize(fromJson.subordinateRotationTransposedNumerics);

        return fromJson;
    }
}
