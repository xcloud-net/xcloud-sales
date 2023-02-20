import _ from './lodash';
import config from './config';
import utils from './utils';
import { StorageMetaDto } from './models';

interface IStorageOption {
  width?: number;
  height?: number;
}

const resolveObjectUrl = function(
  data: StorageMetaDto,
  option?: IStorageOption,
): string | null {
  if (!data) {
    return null;
  }

  const { ResourceKey, StorageProvider } = data;
  if (_.isEmpty(ResourceKey) || _.isEmpty(StorageProvider)) {
    return null;
  }

  /*
  * 目前后端只提供一种storage provider，因此这里的判断其实没有必要。
  * 这里提供一种方式，用来在多provider的情况下扩展
  * */

  if (StorageProvider == 'abp-fs-blob-provider-1') {
    //resolve abp fs url
  }

  if (StorageProvider == 'qcloud') {
    //resolve qcloud url
  }

  //这里是默认resolver
  var resizeOption = `${option?.width || 0}x${option?.height || 0}`;
  var path = `/api/platform/storage/file/${resizeOption}/`;
  var url = utils.concatUrl([config.apiGateway, path, ResourceKey || '']);
  return url;
};

const resolveStringUrl = (data: string, option?: IStorageOption): string | null => {
  if (_.isEmpty(data)) {
    return null;
  }
  if (
    _.startsWith(data.toLowerCase(), 'https://') ||
    _.startsWith(data.toLowerCase(), 'http://')
  ) {
    return data;
  } else if (_.startsWith(data, '{') && _.endsWith(data, '}')) {
    try {
      return resolveObjectUrl(JSON.parse(data), option);
    } catch (e) {
      console.log(e);
    }
  } else {
    return utils.concatUrl([config.apiGateway, data]);
  }
  return null;
};

const resolveUrlv2 = function(data: any, option?: IStorageOption): string | null {
  if (!data) {
    return null;
  }

  if (_.isString(data)) {
    return resolveStringUrl(data, option);
  } else {
    return resolveObjectUrl(data, option);
  }
};

const resolveAvatar = (avatar: any, option?: IStorageOption): string => {
  var url = resolveUrlv2(avatar, option);
  if (url) {
    return url;
  }

  return utils.concatUrl([
    config.apiGateway,
    '/api/platform/user/random-avatar',
  ]);
};

export default {
  resolveUrlv2,
  resolveAvatar,
  resolveObjectUrl,
  resolveStringUrl,
};
