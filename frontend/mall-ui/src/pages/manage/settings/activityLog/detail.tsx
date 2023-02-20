import { Descriptions } from 'antd';
import { ActivityLogDto } from '@/utils/models';
import u from '@/utils';

const App = (props: { model: ActivityLogDto }) => {
  const { model } = props;
  return (
    <>
      <Descriptions title="日志细节" bordered>
        <Descriptions.Item label="请求地址">
          {model.RequestPath}
        </Descriptions.Item>
        <Descriptions.Item label="ip地址">{model.IpAddress}</Descriptions.Item>
        <Descriptions.Item label="位置">
          {[model.GeoCountry, model.GeoCity]
            .filter((x) => !u.isEmpty(x))
            .join('/')}
        </Descriptions.Item>
        <Descriptions.Item label="坐标">{`${model.Lng},${model.Lat}`}</Descriptions.Item>
        <Descriptions.Item label="客户端信息">
          {model.UserAgent}
        </Descriptions.Item>
        <Descriptions.Item label="设备类型">{model.Device}</Descriptions.Item>
        <Descriptions.Item label="浏览器类型" span={2}>
          {model.BrowserType}
        </Descriptions.Item>
        <Descriptions.Item label="请求来源页面" span={3}>
          {model.UrlReferrer}
        </Descriptions.Item>
        <Descriptions.Item label="值">{model.Value}</Descriptions.Item>
        <Descriptions.Item label="详细参数">{model.Data}</Descriptions.Item>
      </Descriptions>
    </>
  );
};

export default App;
