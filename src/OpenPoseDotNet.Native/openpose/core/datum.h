#ifndef _CPP_OP_CORE_DATUM_H_
#define _CPP_OP_CORE_DATUM_H_

#include "../shared.h"

DLLEXPORT op::Datum* op_core_datum_new()
{
    return new op::Datum();
}

DLLEXPORT void op_core_datum_delete(op::Datum* datum)
{
    delete datum;
}

DLLEXPORT unsigned long long op_core_datum_get_id(op::Datum* datum)
{
    return datum->id;
}

DLLEXPORT unsigned long long op_core_datum_get_subId(op::Datum* datum)
{
    return datum->subId;
}

DLLEXPORT unsigned long long op_core_datum_get_subIdMax(op::Datum* datum)
{
    return datum->subIdMax;
}

DLLEXPORT unsigned long long op_core_datum_get_frameNumber(op::Datum* datum)
{
    return datum->frameNumber;
}

DLLEXPORT cv::Mat* op_core_datum_get_cvInputData(op::Datum* datum)
{
    return &datum->cvInputData;
}

DLLEXPORT void op_core_datum_set_cvInputData(op::Datum* datum, cv::Mat* mat)
{
    datum->cvInputData = *mat;
}

DLLEXPORT cv::Mat* op_core_datum_get_cvOutputData(op::Datum* datum)
{
    return &datum->cvOutputData;
}

DLLEXPORT void op_core_datum_set_cvOutputData(op::Datum* datum, cv::Mat* mat)
{
    datum->cvOutputData = *mat;
}

DLLEXPORT op::Array<float>* op_core_datum_get_poseKeypoints(op::Datum* datum)
{
    return &datum->poseKeypoints;
}

DLLEXPORT op::Array<float>* op_core_datum_get_faceKeypoints(op::Datum* datum)
{
    return &datum->faceKeypoints;
}

DLLEXPORT std::array<op::Array<float>, 2>* op_core_datum_get_handKeypoints(op::Datum* datum)
{
    return &datum->handKeypoints;
}

DLLEXPORT op::Array<float>* op_core_datum_get_poseHeatMaps(op::Datum* datum)
{
    return &datum->poseHeatMaps;
}

DLLEXPORT op::Array<float>* op_core_datum_get_faceHeatMaps(op::Datum* datum)
{
    return &datum->faceHeatMaps;
}

DLLEXPORT std::array<op::Array<float>, 2>* op_core_datum_get_handHeatMaps(op::Datum* datum)
{
    return &datum->handHeatMaps;
}

#endif