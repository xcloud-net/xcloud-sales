import u from '@/utils';
import { SysUserDto } from '@/utils/models';
import { Avatar, Popover, Space } from 'antd';

export default (props: { model?: SysUserDto; empty?: any }) => {
  const { model, empty } = props;

  if (model == null) {
    return empty || <></>;
  }

  const avatarUrl = u.resolveAvatar(model.Avatar, { width: 100, height: 100 });

  const nickName = u.simpleString(
    u.firstNotEmpty([model.NickName, model.IdentityName, model.Id]) || '--',
    5,
  );

  const renderDetail = () => {
    return (
      <>
        <Space direction="horizontal">
          <div>
            <Avatar src={avatarUrl} size={100} />
          </div>
          <div>
            <p>昵称：{nickName}</p>
            <p>用户名：{model?.IdentityName || '--'}</p>
            <p>手机号：{model?.AccountMobile || '--'}</p>
          </div>
        </Space>
      </>
    );
  };

  return (
    <>
      <Popover content={renderDetail()}>
        <Space direction="horizontal">
          <Avatar size={'small'} shape="square" src={avatarUrl}>
            {nickName}
          </Avatar>
          <span>
            <a>{nickName}</a>
          </span>
        </Space>
      </Popover>
    </>
  );
};
