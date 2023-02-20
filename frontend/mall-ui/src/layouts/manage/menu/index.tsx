import u from '@/utils';
import { IRoute } from '@/utils/models';
import { Menu } from 'antd';
import { useEffect, useState } from 'react';
import { history } from 'umi';
import MenuData from './data';
import utils from './utils';

interface XProps {
  children: any;
  items?: IRoute[];
}

const App = (props: XProps) => {
  const { children, items } = props;
  const [data, _data] = useState<IRoute[]>([]);
  const [selectedKeys, _selectedKeys] = useState<string[]>([]);

  useEffect(() => {
    const selectedMenu = u.find(MenuData, (x) =>
      utils.checkIsSelected(x, history.location.pathname, null),
    );
    console.log('selected menu', selectedMenu);
    _data(selectedMenu?.routes || []);
    _selectedKeys(utils.resolveSelectedMenu(history.location.pathname));
  }, [history.location.pathname]);

  useEffect(() => {
    u.isEmpty(items) || _data(items || []);
  }, []);

  return (
    <div style={{}}>
      <div style={{ marginBottom: 10 }}>
        <Menu mode="horizontal" selectedKeys={selectedKeys}>
          {u.map(data, (x, index) => {
            return (
              <Menu.Item
                key={x.path || index}
                icon={x.icon}
                onClick={() => {
                  x.path &&
                    history.push({
                      pathname: x.path,
                    });
                }}
              >
                {x.name}
              </Menu.Item>
            );
          })}
        </Menu>
      </div>
      <div style={{}}>{children}</div>
    </div>
  );
};

export default App;
