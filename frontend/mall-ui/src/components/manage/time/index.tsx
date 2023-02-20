import u from '@/utils';
import { Tooltip } from 'antd';

export default (props: {
  model?: any;
  creationTimeKey?: any;
  updationTimeKey?: any;
}) => {
  const { model, creationTimeKey, updationTimeKey } = props;

  const renderTime = (timeStr: string) => {
    var formatedTime = u.formatDateTime(timeStr);
    var relativeTime = u.dayjs(timeStr).add(u.timezoneOffset, 'hour').fromNow();
    return (
      <Tooltip title={formatedTime}>
        <span>{relativeTime}</span>
      </Tooltip>
    );
  };

  if (model == null) {
    return <></>;
  }

  const creationTime = (creationTimeKey || ((x: any) => x.CreationTime))(model);
  const updateTime = (creationTimeKey || ((x: any) => x.LastModificationTime))(
    model,
  );

  return (
    <>
      <div style={{}}>
        {u.isEmpty(creationTime) || (
          <div>创建时间：{renderTime(creationTime)}</div>
        )}
        {u.isEmpty(updateTime) || <div>修改时间：{renderTime(updateTime)}</div>}
      </div>
    </>
  );
};
