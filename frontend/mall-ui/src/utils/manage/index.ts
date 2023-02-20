import u from '@/utils';
import http from '@/utils/http';
import { message } from 'antd';
import Compressor from 'compressorjs';
import heic2any from 'heic2any';
import { PictureDto, StorageMetaDto, ApiResponse } from '@/utils/models';

const allowedImageExtensions = () => ['jpg', 'jpeg', 'png', 'gif', 'heic'];

const isImage = (filename: string) => {
  var ext = u.lowerCase(u.getFileExtension(filename));

  var is_image = allowedImageExtensions().indexOf(ext) >= 0;
  return is_image;
};

const compressImage = (file: File) => {
  return new Promise<File>((resolve, reject) => {
    const compressor = new Compressor(file, {
      maxHeight: 1000,
      maxWidth: 1000,
      quality: 1,
      success: (result: File) => {
        resolve(result);
      },
      error: (error: Error) => {
        reject(error);
      },
    });
    console.log(compressor);
  });
};

const convertFileV2 = (file: File) => {
  console.log('convert file:', file);

  var filename = file.name || '';
  var extension = u.lowerCase(u.getFileExtension(filename));
  var nameWithoutExtension = u.trimEnd(filename, `.${extension}`);

  return new Promise<File>((resolve, reject) => {
    if (extension == 'heic') {
      message.info('正在转换苹果图片格式...');
      heic2any({ blob: file, toType: 'image/jpeg' })
        .then((res: any) => {
          var f = new File([res], `${nameWithoutExtension}.jpg`, {
            type: 'image/jpeg',
          });
          resolve(f);
        })
        .catch((e) => {
          message.error('转换苹果格式错误');
          reject(e);
        });
    } else {
      resolve(file);
    }
  });
};

const uploadFileAndSavePicture = (file: File) => {
  return new Promise<PictureDto | undefined>(async (resolve, reject) => {
    try {
      var res = await uploadFileV3(file);
      if (res.Error) {
        reject(res.Error?.Message);
        return;
      }
      var data = u.first(res.Data || []);
      if (!data) {
        reject('response error');
        return;
      }

      var pic = {
        MimeType: file.type,
        SeoFilename: file.name,
        StorageMeta: data,
      };

      var pictureResponse = await http.apiRequest.post(
        '/mall/common/save-pictures',
        [pic],
      );
      var pictureDto = u.first(pictureResponse.data.Data || []);
      if (!pictureDto) {
        reject('picture dto not exist');
        return;
      }
      resolve(pictureDto);
    } catch (e) {
      reject(e);
    }
  });
};

const uploadFileV3 = (file: File) => {
  return new Promise<ApiResponse<StorageMetaDto[]>>(async (resolve, reject) => {
    try {
      var p = new FormData();
      p.append('f', file, file.name);

      var res: {
        data: ApiResponse<StorageMetaDto[]>;
      } = await http.apiRequest.post('/platform/qcloud-fs/upload', p);

      resolve(res.data || {});
    } catch (e) {
      reject(e);
    }
  });
};

const fileAsBase64 = (file: File) => {
  return new Promise<string>((resolve, reject) => {
    var reader = new FileReader();
    reader.onload = function () {
      resolve(reader.result as string);
    };
    reader.onerror = function (error) {
      reject(error);
    };
    reader.readAsDataURL(file);
  });
};

export default {
  allowedImageExtensions,
  isImage,
  compressImage,
  convertFileV2,
  fileAsBase64,
  uploadFileAndSavePicture,
};
