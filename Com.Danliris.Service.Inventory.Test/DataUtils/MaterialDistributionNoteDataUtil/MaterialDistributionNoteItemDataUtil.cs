using Com.Danliris.Service.Inventory.Lib.Services;
using MRN = Com.Danliris.Service.Inventory.Test.DataUtils.MaterialRequestNoteDataUtil;
using System;
using System.Collections.Generic;
using System.Text;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialsRequestNoteModel;
using Com.Danliris.Service.Inventory.Lib.Models.MaterialDistributionNoteModel;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Test.DataUtils.MaterialDistributionNoteDataUtil
{
    //public class MaterialDistributionNoteItemDataUtil
    //{
    //    private readonly MRN.MaterialRequestNoteDataUtil materialRequestNoteDataUtil;
    //    private readonly MaterialDistributionNoteDetailDataUtil materialDistributionNoteDetailDataUtil;

    //    public MaterialDistributionNoteItemDataUtil(MRN.MaterialRequestNoteDataUtil materialRequestNoteDataUtil, MaterialDistributionNoteDetailDataUtil materialDistributionNoteDetailDataUtil)
    //    {
    //        this.materialRequestNoteDataUtil = materialRequestNoteDataUtil;
    //        this.materialDistributionNoteDetailDataUtil = materialDistributionNoteDetailDataUtil;
    //    }

    //    public MaterialDistributionNoteItem GetNewData()
    //    {
    //        Task<MaterialsRequestNote> materialsRequestNote = Task.Run(() => materialRequestNoteDataUtil.GetTestData());
    //        materialsRequestNote.Wait();

    //        return new MaterialDistributionNoteItem
    //        {
    //            MaterialRequestNoteId = materialsRequestNote.Result.Id,
    //            MaterialRequestNoteCode = materialsRequestNote.Result.Code,
    //            MaterialRequestNoteCreatedDateUtc = materialsRequestNote.Result._CreatedUtc,
    //            MaterialDistributionNoteDetails = materialDistributionNoteDetailDataUtil.GetNewData(materialsRequestNote.Result)
    //        };
    //    }
    //}
}
