import avatar_url from '@/assets/static/images/avatars/avatar_21.jpg';
import u from '@/utils';
import {
  EditOutlined,
  LogoutOutlined,
  SettingOutlined,
  UserOutlined,
} from '@ant-design/icons';
import {
  Avatar,
  Button,
  Card,
  message,
  Popover,
  Skeleton,
  Tooltip,
} from 'antd';
import { useModel } from 'umi';

var index = (props: any) => {
  const storeAppAccountModel = useModel('storeAppAccount');

  var avatarIcon =
    u.resolveAvatar(storeAppAccountModel.StoreAdmin.SysUser?.Avatar, {
      width: 100,
      height: 100,
    }) || avatar_url;

  var renderCard = () => {
    return (
      <Card
        style={{ width: 300, margin: 0, padding: 0 }}
        actions={[
          <SettingOutlined
            key="setting"
            onClick={() => {
              message.error('未实现功能');
            }}
          />,
          <EditOutlined
            key="edit"
            onClick={() => {
              message.error('未实现功能');
            }}
          />,
          <Tooltip title="退出登录" placement="bottom">
            <LogoutOutlined
              key="ellipsis"
              onClick={() => {
                if (confirm('退出登录？')) {
                  u.setAccessToken('');
                  u.redirectToLogin();
                }
              }}
            />
          </Tooltip>,
        ]}
      >
        <Skeleton loading={false} avatar active>
          <Card.Meta
            avatar={<Avatar src={avatarIcon} size={60} shape="square" />}
            title={storeAppAccountModel.StoreAdmin.SysUser?.NickName || '--'}
            description={`User Name: ${
              storeAppAccountModel.StoreAdmin?.SysUser?.IdentityName || '--'
            }`}
          />
        </Skeleton>
      </Card>
    );
  };

  const renderPopover = () => {
    var card = renderCard();
    return (
      <Popover
        title={null}
        content={card}
        placement="topRight"
        style={{ padding: 0, margin: 0 }}
        trigger="click"
      >
        <a onClick={() => {}}>
          <Avatar
            shape="square"
            size="small"
            src={avatarIcon}
            icon={<UserOutlined />}
          />
          <Button type="text" size="small">
            {storeAppAccountModel.StoreAdmin.SysUser?.NickName || '--'}
          </Button>
        </a>
      </Popover>
    );
  };

  return <>{storeAppAccountModel.StoreAdmin && renderPopover()}</>;
};

export default index;
