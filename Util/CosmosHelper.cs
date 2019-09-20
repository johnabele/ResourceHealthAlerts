using System;
using System.Collections.Generic;
using System.Text;
using ResourceHealthAlertPOC.Models;

namespace ResourceHealthAlertPOC.Util
{
    public static class CosmosHelper
    {
        public static ResourceHealthDto GetDtoMapping(ResourceHealthAlert resource)
        {
            var resourceDto = new ResourceHealthDto
            {
                id = Guid.NewGuid().ToString(),
                alertId = resource.data.essentials.alertId,
                alertStatus = resource.data.alertContext.status,
                resourceId = resource.data.essentials.alertTargetIDs[0],
                currentHealthStatus = resource.data.alertContext.properties.currentHealthStatus,
                previousHealthStatus = resource.data.alertContext.properties.previousHealthStatus,
                eventTimestamp = resource.data.alertContext.eventTimestamp,
                subscriptionId = resource.data.alertContext.correlationId
            };

            return resourceDto;
        }
    }
}
