import { Space, Tag } from 'antd';
import XCache from './cache';
import XUser from './user';

var index = (props: any) => {
  return (
    <>
      <div
        style={{
          display: 'flex',
          flexDirection: 'row',
          alignItems: 'center',
          justifyContent: 'space-between',
          paddingRight: '20px',
        }}
      >
        <Tag color="blue" style={{ marginLeft: 20 }}>
          <a href="/store" target="_blank">
            查看店铺
          </a>
        </Tag>
        <Space direction="horizontal">
          <XCache />
          <XUser />
        </Space>
      </div>
    </>
  );
};

export default index;
