import _ from './lodash';

const convertTreeNodesForTreeSelect: any = (data: any[]) => {
  var handledKeys: any[] = [];

  const convertTreeNodes: any = (treeNodes: any[]) => {
    if (_.isEmpty(treeNodes)) {
      return [];
    }

    treeNodes = _.filter(treeNodes, (x) => handledKeys.indexOf(x.key) < 0);

    treeNodes.forEach((node: any) => {
      handledKeys.push(node.key);
    });

    return _.map(treeNodes, (x) => ({
      ...x,
      value: x.key,
      children: convertTreeNodes(x.children),
    }));
  };

  return convertTreeNodes(data);
};

const selectedTenantIdKey = '_selectedTenantId';

const setSelectedTenant = (tenantId: string) => {
  localStorage.setItem(selectedTenantIdKey, tenantId);
};

const getSelectedTenantId = () => {
  return localStorage.getItem(selectedTenantIdKey);
};

const findTreeNodes = (nodes: any[], callback: Function) => {
  var ids: String[] = [];

  var findNode = (n: any) => {
    if (!n || !n.key || ids.indexOf(n.key) >= 0) {
      return;
    }
    ids.push(n.key);

    callback(n);

    if (!_.isEmpty(n.children)) {
      n.children.map((x: any) => findNode(x));
    }
  };

  nodes.map((x: any) => findNode(x));
};

const getTreeKeys: any = (nodes: any[]) => {
  var ids: any[] = [];

  findTreeNodes(nodes, (x: any) => {
    ids.push(x.key);
  });

  return ids;
};

const getNodesByKeys: any = (nodes: any[], keys: any[]) => {
  var data: any[] = [];

  findTreeNodes(nodes, (x: any) => {
    if (keys.indexOf(x.key) >= 0) {
      data.push(x);
    }
  });

  return data;
};

const isNodeMatch = (node: any, q: String) => {
  var title = node?.title?.toLowerCase();
  q = q?.toLowerCase();

  return q && q.length > 0 && title && title.indexOf(q) >= 0;
};

const searchTreeNodes = (
  originData: any,
  queryStr: String,
  onMatchedKeys: Function,
  onFilteredData: Function,
  beforeFunc: Function,
  afterFunc: Function,
) => {
  if (beforeFunc) {
    beforeFunc();
  }

  var q = queryStr || '';
  q = q.trim();

  var matchFunction = isNodeMatch;

  console.log('origin data', originData);
  console.log('search:', q);

  var visitedKeys: any[] = [];
  var expandKeys: any[] = [];

  var searchFunction = (node: any) => {
    if (!node || visitedKeys.indexOf(node.key) >= 0) {
      return;
    }
    visitedKeys.push(node.key);

    node.match = false;
    node.expand = false;
    node.children = node.children || [];

    //先查找子节点
    node.children = node.children.map(searchFunction).filter((x: any) => x);
    //子节点有匹配就展开
    node.expand = node.children.some((x: any) => x.match);
    //判断当前节点是否匹配
    node.match = matchFunction(node, q);

    if (node.expand) {
      expandKeys.push(node.key);
    }

    return node;
  };

  var filteredData = originData.map((x: any) => searchFunction(x)).filter((x: any) => x);
  console.log('expand keys:', expandKeys);

  onFilteredData(filteredData);
  onMatchedKeys(expandKeys);

  if (afterFunc) {
    afterFunc();
  }
};

export default {
  searchTreeNodes,
  isNodeMatch,
  getNodesByKeys,
  getTreeKeys,
  findTreeNodes,
  getSelectedTenantId,
  setSelectedTenant,
  convertTreeNodesForTreeSelect,
};
